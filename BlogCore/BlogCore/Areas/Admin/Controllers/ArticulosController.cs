using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticulosController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingenviroment;

        public ArticulosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingenviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingenviroment = hostingenviroment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ArticuloVM articuloVM = new ArticuloVM()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            return View(articuloVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloVM artivm)
        {   //todo esto solo para subir una imagen
            if (ModelState.IsValid)
            {
                string rutaprincipal = _hostingenviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if (artivm.Articulo.Id == 0)
                {
                    //nuevo articulo
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaprincipal, @"imagenes\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStream);
                    }

                    artivm.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + extension;
                    artivm.Articulo.FechaCreacion = DateTime.Now.ToString();

                    _contenedorTrabajo.Articulo.Add(artivm.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
            }
            artivm.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();

            return View(artivm);
        }



        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ArticuloVM articuloVM = new ArticuloVM()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            if (id != null)
            {
                articuloVM.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());
            }

            return View(articuloVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticuloVM artivm)
        {
            if (ModelState.IsValid)
            {
                string rutaprincipal = _hostingenviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var articuloDesdedb = _contenedorTrabajo.Articulo.Get(artivm.Articulo.Id);

                if (archivos.Count() > 0)
                {
                    //editamos imagenes
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaprincipal, @"imagenes\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);
                    var nuevaExtencion = Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(rutaprincipal, articuloDesdedb.UrlImagen.TrimStart('\\'));

                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    //subimos nuevamente el archivo
                    using (var fileStream = new FileStream(Path.Combine(subidas, nombreArchivo + nuevaExtencion), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStream);
                    }

                    artivm.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + nuevaExtencion;
                    artivm.Articulo.FechaCreacion = DateTime.Now.ToString();

                    _contenedorTrabajo.Articulo.Update(artivm.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //Aqui es cuando la imagen ya existe y no se remplaza
                    //Debe concervar la que ya tiene en la DB
                    artivm.Articulo.UrlImagen = articuloDesdedb.UrlImagen;
                }

                _contenedorTrabajo.Articulo.Update(artivm.Articulo);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(id);
            string rutaDirectorioprincipal = _hostingenviroment.WebRootPath;
            var rutaImagen = Path.Combine(rutaDirectorioprincipal, articuloDesdeDb.UrlImagen.TrimStart('\\'));

            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
            }

            if (articuloDesdeDb == null)
            {
                return Json(new { success = false, message = "Error borrando articulo"});
            }

            _contenedorTrabajo.Articulo.Remove(articuloDesdeDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Articulo borrado" });


        }

        #region LLAMADAS A LA API

        [HttpGet]
        public IActionResult Getall()
        {
            return Json(new { data = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria") });
        }


        

        #endregion
    }
}
