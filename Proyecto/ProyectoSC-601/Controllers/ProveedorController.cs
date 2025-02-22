﻿using ProyectoSC_601.Entities;
using ProyectoSC_601.Models;
using System;
using System.Web.Mvc;
using WEB_ImpoMoyaUlate.Filters;



namespace ProyectoSC_601.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class ProveedorController : Controller
    {

        ProveedorModel modelProveedor = new ProveedorModel();



        /*Esto devuelve la vista para registrar un proveedor*/
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult RegistrarProveedor()
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        ViewBag.combo = modelProveedor.ConsultarEmpresas();
                        ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                        return View();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.combo = modelProveedor.ConsultarEmpresas();
                        return View("Error");
                    }

                }

                else
                {
                    return View();
                }

            }

            else
            {
                return View();
            }
        }




        /*Se llama cuando se envía el formulario para registrar un proveedor*/
        [AuthorizeRol(1)]
        [HttpPost]
        public ActionResult RegistrarProveedor(ProveedorEnt entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string cedulaExistente = modelProveedor.ComprobarCedulaProveedor(entidad);

                    if (cedulaExistente == "Existe")
                    {
                        ViewBag.MensajeCedulaExistente = "El proveedor con esta cédula ya está registrado.";
                        ViewBag.combo = modelProveedor.ConsultarEmpresas();
                        ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                        return View();
                    }

                    // Continuar con el registro solo si la cédula no existe
                    string respuesta = modelProveedor.RegistrarProveedor(entidad);

                    if (respuesta == "OK")
                    {
                        TempData["RegistroExito"] = "El proveedor se registró correctamente.";
                        return RedirectToAction("ConsultaProveedores", "Proveedor");
                    }
                    else
                    {
                        ViewBag.MensajeUsuario = "No se ha podido registrar la información del proveedor";
                        ViewBag.combo = modelProveedor.ConsultarEmpresas();
                        ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                        return View();
                    }
                }
                else
                {

                    ViewBag.combo = modelProveedor.ConsultarEmpresas();
                    ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }



        /* Se llama cuando se solicita la página de consulta de proveedores para mostrar los datos de todos los proveedores*/
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ConsultaProveedores()
        {
            try
            {
                var datos = modelProveedor.ConsultaProveedores();
                ViewBag.combo = modelProveedor.ConsultarEmpresas();
                ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                return View(datos);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }



        /*Se llama cuando se desea actualizar el estado de un proveedor.*/
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ActualizarEstadoProveedor(long q)
        {
            try
            {
                var entidad = new ProveedorEnt();
                entidad.ID_Proveedor = q;
                ViewBag.combo = modelProveedor.ConsultarEmpresas();
                ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                string respuesta = modelProveedor.ActualizarEstadoProveedor(entidad);

                if (respuesta == "OK")
                {
                    return RedirectToAction("ConsultaProveedores", "Proveedor");
                }
                else
                {
                    ViewBag.combo = modelProveedor.ConsultarEmpresas();
                    ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                    ViewBag.MensajeUsuario = "No se ha podido cambiar el estado del proveedor";
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }



        /* Se llama cuando se desea actualizar la información de un proveedor modificada
           Este sirve para Visualizar los datos del proveedor y la lista de empresas. */
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ActualizarProveedor(long q)
        {
            try
            {
                var datos = modelProveedor.ConsultaProveedor(q);
                ViewBag.combo = modelProveedor.ConsultarEmpresas();
                ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                return View(datos);
            }
            catch (Exception ex)
            {
                ViewBag.combo = modelProveedor.ConsultarEmpresas();
                ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                ModelState.AddModelError(string.Empty, "Error al cargar los datos del proveedor.");
                return View();
            }
        }

        /* Este procesa la actualización de datos de un proveedor desde un formulario y redirige. */
        [AuthorizeRol(1)]
        [HttpPost]
        public ActionResult ActualizarProveedor(ProveedorEnt entidad)
        {
            try
            {
                if (string.IsNullOrEmpty(entidad.Apellido_Proveedor))
                {
                    entidad.Apellido_Proveedor = string.Empty;
                    Console.WriteLine("Apellido establecido como cadena vacía");
                }

                // Almacena el modelo original en TempData antes de intentar la actualización
                TempData["OriginalModel"] = entidad;

                // Realiza la validación para asegurar que el modelo no sea nulo
                if (ModelState.IsValid && entidad != null)
                {
                    string respuesta = modelProveedor.ActualizarProveedor(entidad);

                    if (respuesta == "OK")
                    {
                        TempData["ActualizacionExito"] = "Proveedor actualizado con éxito";
                        return RedirectToAction("ConsultaProveedores", "Proveedor");
                    }
                }

                // Recupera el modelo original de TempData para mostrar los datos en la vista
                ProveedorEnt originalModel = TempData["OriginalModel"] as ProveedorEnt;
                ViewBag.MensajeUsuario = "No se ha podido actualizar la información del proveedor";
                ViewBag.combo = modelProveedor.ConsultarEmpresas();
                ViewBag.Identificaciones = modelProveedor.ConsultarIdentificacionesProveedor();
                return View(originalModel);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }




        /*Se llama cuando se solicita eliminar un proveedor.*/
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult EliminarProveedor(long q)
        {
            try
            {
                string respuesta = modelProveedor.EliminarProveedor(q);

                // Imprime la respuesta en la consola para depuración
                Console.WriteLine($"Respuesta del servicio: {respuesta}");

                if (respuesta == "OK")
                {
                    TempData["ActualizacionExito"] = "Proveedor eliminado con éxito";
                    return RedirectToAction("ConsultaProveedores", "Proveedor");
                }
                else
                {
                    ViewBag.MensajeUsuario = "No se ha podido eliminar el proveedor.";
                    return View("ConsultaProveedores", "Proveedor");
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }





    }
}