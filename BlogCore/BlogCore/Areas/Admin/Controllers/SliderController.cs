using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models;
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
    public class SliderController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _iwebHostEnviroment;

        public SliderController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _iwebHostEnviroment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {   //todo esto solo para subir una imagen
            if (ModelState.IsValid)
            {
                string rutaprincipal = _iwebHostEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                //nuevo articulo
                string nombreArchivo = Guid.NewGuid().ToString();
                var subidas = Path.Combine(rutaprincipal, @"imagenes\sliders");
                var extension = Path.GetExtension(archivos[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStream);
                }

                slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + extension;
                //slider.Articulo.FechaCreacion = DateTime.Now.ToString();

                _contenedorTrabajo.Slider.Add(slider);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }
            

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id != null)
            {
                var slider = _contenedorTrabajo.Slider.Get(id.GetValueOrDefault());
                return View(slider);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (ModelState.IsValid)
            {
                string rutaprincipal = _iwebHostEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var articuloDesdedb = _contenedorTrabajo.Slider.Get(slider.Id);

                if (archivos.Count() > 0)
                {
                    //editamos imagenes
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaprincipal, @"imagenes\sliders");
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

                    slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + nuevaExtencion;
                    

                    _contenedorTrabajo.Slider.Update(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //Aqui es cuando la imagen ya existe y no se remplaza
                    //Debe concervar la que ya tiene en la DB
                    slider.UrlImagen = articuloDesdedb.UrlImagen;
                }

                _contenedorTrabajo.Slider.Update(slider);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }


        #region
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Slider.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Slider.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error borrando el slider" });
            }
            _contenedorTrabajo.Slider.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Slider borrado con exito" });
        }

        #endregion
    }
}
