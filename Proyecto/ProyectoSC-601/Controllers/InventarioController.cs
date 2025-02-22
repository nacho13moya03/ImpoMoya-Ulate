﻿using ProyectoSC_601.Entities;
using ProyectoSC_601.Models;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WEB_ImpoMoyaUlate.Filters;



namespace ProyectoSC_601.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class InventarioController : Controller
    {

        InventarioModel modelInventario = new InventarioModel();


        /* Consulta todos los productos registrados en el sistema */
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ConsultaInventario()
        {
            ViewBag.TotalInventario = modelInventario.ContarTotalInventario();
            var datos = modelInventario.ConsultarInventario();
            return View(datos);
        }

        //Devuele la vista de registrar productos con las categorias 
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult RegistrarProducto()
        {
            ViewBag.Categorias = modelInventario.ConsultarCategorias();
            return View();
        }

        //Registra un producto 
        [AuthorizeRol(1)]
        [HttpPost]
        public ActionResult RegistrarProducto(HttpPostedFileBase Imagen_Nueva, InventarioEnt entidad)
        {
            ModelState.Remove("Imagen");
            ModelState.Remove("SKU");

            int maxFileSize = 6 * 1024 * 1024;

            if (Imagen_Nueva != null && Imagen_Nueva.ContentLength > maxFileSize)
            {
                ViewBag.Categorias = modelInventario.ConsultarCategorias();
                ViewBag.MensajeNoExitoso = "El tamaño de la imagen no debe exceder los 6MB.";
                return View(entidad);
            }

            entidad.Imagen = string.Empty;
            entidad.Estado = 1;
            entidad.Imagen_Nueva = null;

            try
            {
                if (ModelState.IsValid)
                {
                    string skuExistente = modelInventario.ComprobarSKUExistente(entidad);
                    if (skuExistente == "Existe")
                    {
                        ViewBag.Categorias = modelInventario.ConsultarCategorias();
                        ViewBag.Mensaje = "No se ha podido registrar el producto, SKU Repetido";
                        return View();
                    }
                    else
                    {
                        long ID_Producto = modelInventario.RegistrarProducto(entidad);
                        string extension = Path.GetExtension(Path.GetFileName(Imagen_Nueva.FileName));
                        string ruta = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + ID_Producto + extension;
                        Imagen_Nueva.SaveAs(ruta);

                        entidad.Imagen = "/Images/" + ID_Producto + extension;
                        entidad.ID_Producto = ID_Producto;

                        modelInventario.ActualizarRutaProducto(entidad);

                        return RedirectToAction("ConsultaInventario", "Inventario");
                    }

                }

                else
                {
                    ViewBag.Categorias = modelInventario.ConsultarCategorias();
                    return View(entidad);
                }
            }
            catch (HttpException ex) when (ex.WebEventCode == System.Web.Management.WebEventCodes.RuntimeErrorPostTooLarge)
            {
                ViewBag.Categorias = modelInventario.ConsultarCategorias();
                ViewBag.MensajeNoExitoso = "El tamaño del archivo no debe exceder los 6MB.";
                return View(entidad);
            }

        }

        //Cambia el estado de un producto
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ActualizarEstadoProducto(long q)
        {
            var entidad = new InventarioEnt();
            entidad.ID_Producto = q;

            string respuesta = modelInventario.ActualizarEstadoProducto(entidad);

            if (respuesta == "OK")
            {
                return RedirectToAction("ConsultaInventario", "Inventario");
            }
            else
            {
                ViewBag.Mensaje = "No se ha podido cambiar el estado del producto";
                return View();
            }
        }


        //Cambia el estado de un producto
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult EliminarProducto(long q)
        {
            var entidad = new InventarioEnt();
            entidad.ID_Producto = q;

            string respuesta = modelInventario.EliminarProducto(entidad);

            if (respuesta == "OK")
            {
                return RedirectToAction("ConsultaInventario", "Inventario");
            }
            else
            {
                ViewBag.Mensaje = "No se ha podido eliminar el producto";
                return View();
            }
        }


        //Muestra una vista con los datos del producto seleccionado para modificarlo
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ModificarProducto(long q)
        {
            var datos = modelInventario.ConsultaProductoEspecifico(q);
            ViewBag.Categorias = modelInventario.ConsultarCategorias();
            return View(datos);
        }


        //Actualiza el producto con los nuevos datos ingresados
        [AuthorizeRol(1)]
        [HttpPost]
        public ActionResult ModificarProducto(HttpPostedFileBase Imagen_Nueva, InventarioEnt entidad)
        {

            ModelState.Remove("Imagen");
            ModelState.Remove("Imagen_Nueva");

            if (ModelState.IsValid)
            {

                int maxFileSize = 6 * 1024 * 1024;

                if (Imagen_Nueva != null && Imagen_Nueva.ContentLength > maxFileSize)
                {
                    ViewBag.Categorias = modelInventario.ConsultarCategorias();
                    ViewBag.MensajeNoExitoso = "El tamaño de la imagen no debe exceder los 6MB.";
                    return View(entidad);
                }
                try
                {
                    entidad.Imagen_Nueva = null;

                    if (Imagen_Nueva != null)
                    {
                        string extension = Path.GetExtension(Imagen_Nueva.FileName);
                        string rutaNuevaImagen = Path.Combine(Server.MapPath("~/Images/"), entidad.ID_Producto + extension);

                        Imagen_Nueva.SaveAs(rutaNuevaImagen);

                        entidad.Imagen = "/Images/" + entidad.ID_Producto + extension;
                    }

                    long ID_Producto = modelInventario.ActualizarProducto(entidad);

                    if (ID_Producto > 0)
                    {
                        return RedirectToAction("ConsultaInventario", "Inventario");
                    }
                    else
                    {
                        ViewBag.Mensaje = "No se ha podido actualizar el producto";
                        return View();
                    }
                }
                catch (HttpException ex) when (ex.WebEventCode == System.Web.Management.WebEventCodes.RuntimeErrorPostTooLarge)
                {
                    ViewBag.Categorias = modelInventario.ConsultarCategorias();
                    ViewBag.MensajeNoExitoso = "El tamaño del archivo no debe exceder los 6MB.";
                    return View(entidad);
                }
            }
            else
            {
                ViewBag.Categorias = modelInventario.ConsultarCategorias();
                return View(entidad);
            }
        }

        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult VerificarEliminarProducto(int idProducto)
        {
            var entidad = new InventarioEnt { ID_Producto = idProducto };

            bool facturasVinculadas = modelInventario.VerificarFacturasVinculadas(entidad);

            if (facturasVinculadas)
            {
                return Json(new { success = false, message = "No se puede eliminar el producto por información relacionada en las facturas" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: Categoria
        [AuthorizeRol(1)]
        public ActionResult ConsultarCategoria()
        {
            return View();
        }

        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult VerificarCarritoVinculado(int idProducto)
        {
            var entidad = new InventarioEnt { ID_Producto = idProducto };

            bool carritoVinculado = modelInventario.VerificarCarritoVinculado(entidad);

            if (carritoVinculado)
            {
                return Json(new { success = false, message = "No se puede eliminar el producto porque se encuentra en el carrito de un cliente" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}