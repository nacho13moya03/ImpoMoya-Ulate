﻿using ProyectoSC_601.Entities;
using ProyectoSC_601.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WEB_ImpoMoyaUlate.Filters;
using WEB_ImpoMoyaUlate.Models;

namespace ProyectoSC_601.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class UsuarioController : Controller
    {

        UsuarioModel modelUsuario = new UsuarioModel();
        IndexModel modelIndex = new IndexModel();
        Seguridad seguridad = new Seguridad();

        [HttpGet]
        public ActionResult RegistroUsuario()
        {
            ViewBag.Identificaciones = modelUsuario.ConsultarTiposIdentificaciones();
            return View();
        }

        //Se llama al modelo para registrar al cliente y se devuelven mensajes de exito o error
        [HttpPost]
        public ActionResult RegistroUsuario(UsuarioEnt entidad)
        {
            ModelState.Remove("NuevaContrasenna_Usuario");
            ModelState.Remove("ID_Provincia");
            ModelState.Remove("ID_Canton");
            ModelState.Remove("ID_Distrito");
            ModelState.Remove("Direccion_Exacta");
            ModelState.Remove("Telefono_Usuario");
            if (ModelState.IsValid)
            {
                string cedulaExistente = modelUsuario.ComprobarCedulaExistente(entidad);
                if (cedulaExistente == "Existe")
                {
                    ViewBag.MensajeNoExitoso = "El usuario ya está registrado";
                    ViewBag.Identificaciones = modelUsuario.ConsultarTiposIdentificaciones();
                    return View();
                }
                else
                {
                    string correoExistente = modelUsuario.ComprobarCorreoExistenteUsuario(entidad);
                    if (correoExistente == "Existe")
                    {
                        ViewBag.MensajeNoExitoso = "Ese correo está asociado a otra cuenta";
                        ViewBag.Identificaciones = modelUsuario.ConsultarTiposIdentificaciones();
                        return View();
                    }
                    else
                    {
                        entidad.Contrasenna_Usuario = seguridad.Encrypt(entidad.Contrasenna_Usuario);
                        string respuesta = modelUsuario.RegistroUsuario(entidad);

                        if (respuesta == "OK")
                        {
                            ModelState.Clear();
                            ViewBag.MensajeExitoso = "La información se ha registrado exitosamente";
                            ViewBag.Identificaciones = modelUsuario.ConsultarTiposIdentificaciones();
                            return View();
                        }
                        else
                        {
                            ViewBag.MensajeNoExitoso = "No se ha podido registrar la información";
                            ViewBag.Identificaciones = modelUsuario.ConsultarTiposIdentificaciones();
                            return View();
                        }
                    }
                }
            }
            else
            {
                ViewBag.Identificaciones = modelUsuario.ConsultarTiposIdentificaciones();
                return View(entidad);
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            Session.Clear();
            return View();
        }

        // Se llama al modelo para iniciar sesion y redirije a las paginas correspondientes basado en el rol 
        [HttpPost]
        public ActionResult Login(UsuarioEnt entidad)
        {
            ModelState.Remove("Identificacion_Usuario");
            ModelState.Remove("Nombre_Usuario");
            ModelState.Remove("Apellido_Usuario");
            ModelState.Remove("NuevaContrasenna_Usuario");
            ModelState.Remove("ID_Provincia");
            ModelState.Remove("ID_Canton");
            ModelState.Remove("ID_Distrito");
            ModelState.Remove("Direccion_Exacta");
            ModelState.Remove("Telefono_Usuario");
            ModelState.Remove("Contrasenna_Usuario");

            // Luego, aplicar la validación de campo obligatorio manualmente
            if (string.IsNullOrEmpty(entidad.Contrasenna_Usuario))
            {
                ModelState.AddModelError("Contrasenna_Usuario", "Este campo es obligatorio.");
            }

            if (ModelState.IsValid)
            {

                entidad.Contrasenna_Usuario = seguridad.Encrypt(entidad.Contrasenna_Usuario);

                var respuesta = modelUsuario.Login(entidad);

                if (respuesta != null)
                {


                    Session["Contrasena_Temporal"] = respuesta.C_esTemporal;
                    Session["Rol"] = respuesta.ID_Rol;
                    if (respuesta.ID_Rol == 2)
                    {
                        Session["ID_Usuario"] = respuesta.ID_Usuario;
                        return RedirectToAction("Index", "Home");
                    }
                    else if (respuesta.ID_Rol == 1)
                    {
                        Session["ID_Usuario"] = respuesta.ID_Usuario;
                        return RedirectToAction("IndexAdmin", "Home");
                    }
                }

                // Si las credenciales no son válidas, mostrar mensaje de credenciales inválidos
                ViewBag.MensajeNoExitoso = "Credenciales Inválidos o Usuario Inexistente";
                return View();
            }

            // Si el modelo no es válido, volver a mostrar el formulario con los errores
            return View(entidad);
        }


        [HttpGet]
        public ActionResult RecuperarCuentaUsuario()
        {
            return View();
        }


        //Se llama al modelo para recuperar la cuenta, si es exitoso lo redirije al login y si no muestra mensaje de error
        [HttpPost]
        public ActionResult RecuperarCuentaUsuario(UsuarioEnt entidad)
        {
            ModelState.Remove("Nombre_Usuario");
            ModelState.Remove("Apellido_Usuario");
            ModelState.Remove("Identificacion_Usuario");
            ModelState.Remove("Contrasenna_Usuario");
            ModelState.Remove("NuevaContrasenna_Usuario");
            ModelState.Remove("ID_Provincia");
            ModelState.Remove("ID_Canton");
            ModelState.Remove("ID_Distrito");
            ModelState.Remove("Direccion_Exacta");
            ModelState.Remove("Telefono_Usuario");

            if (ModelState.IsValid)
            {
                string respuesta = modelUsuario.RecuperarCuentaUsuario(entidad);

                if (respuesta == "OK")
                {
                    ViewBag.MensajeExitoso = "Se ha enviado un correo con la contraseña temporal";
                    entidad.Correo_Usuario = string.Empty;
                    return View();
                }
                else
                {
                    ViewBag.MensajeNoExitoso = "No se ha podido recuperar su información";
                    return View();
                }

            }
            else
            {
                return View(entidad);
            }
        }

        //Muestra los datos de todos los clientes en el administrador
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult GestionUsuarios()
        {
            long idUsuario = long.Parse(Session["ID_Usuario"].ToString());
            var datos = modelUsuario.ConsultarUsuariosAdministrador(idUsuario);
            return View(datos);
        }


        //Muestra los datos de todos los clientes en el administrador
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult GestionDireccion(long q)
        {
            var datos = modelUsuario.GestionDireccion(q);
            return View(datos);
        }


        public ActionResult cargarCantones(int idProvincia)
        {
            var cantones = modelUsuario.cargarCantones(idProvincia);
            return Json(cantones, JsonRequestBehavior.AllowGet);
        }

        public ActionResult cargarDistritos(int idCanton)
        {
            var distritos = modelUsuario.cargarDistritos(idCanton);
            return Json(distritos, JsonRequestBehavior.AllowGet);
        }

        //Devuelve la vista de perfil con los datos del cliente
        [AuthorizeCliente(2)]
        [HttpGet]
        public ActionResult PerfilCliente()
        {
            try
            {
                if (Session["ID_Usuario"] != null && Session["Rol"] != null && long.Parse(Session["Rol"].ToString()) == 2)
                {
                    // Obtiene la cantidad de productos diferentes en el carrito
                    int cantidadProductos = modelIndex.ObtenerCantidadProductosEnCarrito(long.Parse(Session["ID_Usuario"].ToString()));

                    // Pasa la cantidad de productos a la vista
                    ViewBag.CantidadProductosEnCarrito = cantidadProductos;
                }

                long q = long.Parse(Session["ID_Usuario"].ToString());

                var datos = modelUsuario.ConsultaClienteEspecifico(q);

                // Consultar todas las provincias y cargarlas en el ViewBag
                ViewBag.Provincias = modelUsuario.ConsultarProvincias();

                // Verificar si el usuario tiene una dirección
                if (datos.ID_Direccion != 0)
                {
                    int idProvinciaSeleccionada = datos.ID_Provincia;
                    int idCantonSeleccionado = datos.ID_Canton;
                    int idDistritoSeleccionado = datos.ID_Distrito;

                    // Modificar la lista de provincias para establecer la seleccionada
                    ViewBag.Provincias = ((IEnumerable<SelectListItem>)ViewBag.Provincias)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Value,
                            Text = p.Text,
                            Selected = (p.Value == idProvinciaSeleccionada.ToString())
                        }).ToList();

                    // Consultar los cantones de la provincia seleccionada
                    var cantones = modelUsuario.cargarCantones(idProvinciaSeleccionada);
                    ViewBag.Cantones = cantones
                        .Select(c => new SelectListItem
                        {
                            Value = c.Value,
                            Text = c.Text,
                            Selected = (c.Value == idCantonSeleccionado.ToString())
                        }).ToList();

                    // Consultar los distritos del cantón seleccionado
                    var distritos = modelUsuario.cargarDistritos(idCantonSeleccionado);
                    ViewBag.Distritos = distritos
                        .Select(d => new SelectListItem
                        {
                            Value = d.Value,
                            Text = d.Text,
                            Selected = (d.Value == idDistritoSeleccionado.ToString())
                        }).ToList();
                }
                else
                {
                    // Si no tiene dirección, establecer los cantones y distritos como cadena vacía
                    ViewBag.Cantones = "";
                    ViewBag.Distritos = "";
                }

                return View(datos);
            }
            catch (Exception ex)
            {
                // Si salta esta excepcion es porque un usuario esta intentando ingresar por url entonces lo envia a la vista de no acceso
                return RedirectToAction("NoAcceso", "Seguridad");
            }
        }



        //Actualiza los datos del cliente
        [AuthorizeCliente(2)]
        [HttpPost]
        public ActionResult PerfilCliente(UsuarioEnt entidad)
        {
            ModelState.Remove("Contrasenna_Usuario");
            if (ModelState.IsValid)
            {

                string correoExistente = modelUsuario.ComprobarCorreoExistenteUsuario(entidad);
                if (correoExistente == "Existe")
                {
                    ViewBag.MensajeNoExitoso = "Ese correo está asociado a otra cuenta";
                    long q = long.Parse(Session["ID_Usuario"].ToString());
                    var datos = modelUsuario.ConsultaClienteEspecifico(q);
                    Session["ID_Usuario"] = datos.ID_Usuario;
                    return View(datos);
                }
                else
                {
                    if (entidad.NuevaContrasenna_Usuario == null || entidad.NuevaContrasenna_Usuario.Length == 0)
                    {
                        entidad.NuevaContrasenna_Usuario = entidad.Contrasenna_Usuario;
                    }
                    else
                    {
                        // Encriptar la nueva contraseña antes de guardarla
                        entidad.NuevaContrasenna_Usuario = seguridad.Encrypt(entidad.NuevaContrasenna_Usuario);
                    }

                    if (entidad.Apellido_Usuario == null || entidad.Apellido_Usuario.Length == 0)
                    {
                        entidad.Apellido_Usuario = string.Empty;
                    }

                    string respuesta = modelUsuario.ActualizarCuentaCliente(entidad);

                    if (respuesta == "OK")
                    {
                        Session["Direccion_Cliente"] = entidad.Direccion_Exacta;

                        return RedirectToAction("PerfilCliente", "Usuario");

                    }
                    else
                    {
                        ViewBag.MensajeNoExitoso = "No se ha podido actualizar su información";
                        return View();
                    }

                }
            }
            else
            {
                if (entidad.ID_Direccion != 0)
                {
                    var datos = modelUsuario.ConsultaClienteEspecifico(entidad.ID_Usuario);

                    ViewBag.Provincias = modelUsuario.ConsultarProvincias();

                    if (datos.ID_Direccion != 0)
                    {
                        int idProvinciaSeleccionada = datos.ID_Provincia;
                        int idCantonSeleccionado = datos.ID_Canton;
                        int idDistritoSeleccionado = datos.ID_Distrito;

                        ViewBag.Provincias = ((IEnumerable<SelectListItem>)ViewBag.Provincias)
                            .Select(p => new SelectListItem
                            {
                                Value = p.Value,
                                Text = p.Text,
                                Selected = (p.Value == idProvinciaSeleccionada.ToString())
                            }).ToList();

                        var cantones = modelUsuario.cargarCantones(idProvinciaSeleccionada);
                        ViewBag.Cantones = cantones
                            .Select(c => new SelectListItem
                            {
                                Value = c.Value,
                                Text = c.Text,
                                Selected = (c.Value == idCantonSeleccionado.ToString())
                            }).ToList();

                        var distritos = modelUsuario.cargarDistritos(idCantonSeleccionado);
                        ViewBag.Distritos = distritos
                            .Select(d => new SelectListItem
                            {
                                Value = d.Value,
                                Text = d.Text,
                                Selected = (d.Value == idDistritoSeleccionado.ToString())
                            }).ToList();
                        return View(entidad);
                    }
                }


                ViewBag.Provincias = modelUsuario.ConsultarProvincias();
                int idProvinciaS = entidad.ID_Provincia;
                int idCantonS = entidad.ID_Canton;
                int idDistritoS = entidad.ID_Distrito;

                ViewBag.Provincias = ((IEnumerable<SelectListItem>)ViewBag.Provincias)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Value,
                        Text = p.Text,
                        Selected = (p.Value == idProvinciaS.ToString())
                    }).ToList();

                var cantones2 = modelUsuario.cargarCantones(idProvinciaS);
                ViewBag.Cantones = cantones2
                    .Select(c => new SelectListItem
                    {
                        Value = c.Value,
                        Text = c.Text,
                        Selected = (c.Value == idCantonS.ToString())
                    }).ToList();

                var distritos2 = modelUsuario.cargarDistritos(idCantonS);
                ViewBag.Distritos = distritos2
                    .Select(d => new SelectListItem
                    {
                        Value = d.Value,
                        Text = d.Text,
                        Selected = (d.Value == idDistritoS.ToString())
                    }).ToList();
                return View(entidad);
            }

        }


        //Inactiva el usuario segun el id del cliente recibido

        [HttpGet]
        public ActionResult InactivarUsuario(long q)
        {
            var entidad = new UsuarioEnt();
            entidad.ID_Usuario = q;
            modelUsuario.InactivarUsuario(entidad);
            return RedirectToAction("CerrarSesionUsuario", "Usuario");

        }


        //Limpia los datos de la sesion y lo redirije al index
        [HttpGet]
        public ActionResult CerrarSesionUsuario()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        //Cambia el estado del cliente desde el admnistrador
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ActualizarEstadoUsuario(long q)
        {
            var entidad = new UsuarioEnt();
            entidad.ID_Usuario = q;

            string respuesta = modelUsuario.ActualizarEstadoUsuario(entidad);

            if (respuesta == "OK")
            {
                return RedirectToAction("GestionUsuarios", "Usuario");
            }
            else
            {
                ViewBag.Mensaje = "No se ha podido cambiar el estado del usuario";
                return View();
            }
        }

        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult ActualizarRolUsuario(long q)
        {
            var entidad = new UsuarioEnt();
            entidad.ID_Usuario = q;

            string respuesta = modelUsuario.ActualizarRolUsuario(entidad);

            if (respuesta == "OK")
            {
                return RedirectToAction("GestionUsuarios", "Usuario");
            }
            else
            {
                ViewBag.Mensaje = "No se ha podido cambiar el rol del usuario";
                return View();
            }
        }



        [HttpPost]
        public ActionResult CambiarContrasenaTemporal(string nuevaContrasena)
        {
            var entidad = new UsuarioEnt();
            entidad.ID_Usuario = long.Parse(Session["ID_Usuario"].ToString());
            entidad.Contrasenna_Usuario = nuevaContrasena;

            entidad.Contrasenna_Usuario = seguridad.Encrypt(entidad.Contrasenna_Usuario);
            string respuesta = modelUsuario.ActualizarContrasenaTemporal(entidad);
            if (respuesta == "Ok")
            {
                Session["Contrasena_Temporal"] = 0;
            }
            if (long.Parse(Session["Rol"].ToString()) == 1)
            {
                return RedirectToAction("IndexAdmin", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        //Devuelve la vista de perfil con los datos del cliente
        [AuthorizeRol(1)]
        [HttpGet]
        public ActionResult PerfilAdmin()
        {
            try
            {
                if (Session["ID_Usuario"] != null && Session["Rol"] != null && long.Parse(Session["Rol"].ToString()) == 2)
                {
                    // Obtiene la cantidad de productos diferentes en el carrito
                    int cantidadProductos = modelIndex.ObtenerCantidadProductosEnCarrito(long.Parse(Session["ID_Usuario"].ToString()));

                    // Pasa la cantidad de productos a la vista
                    ViewBag.CantidadProductosEnCarrito = cantidadProductos;
                }

                long q = long.Parse(Session["ID_Usuario"].ToString());

                var datos = modelUsuario.ConsultaClienteEspecifico(q);

                // Consultar todas las provincias y cargarlas en el ViewBag
                ViewBag.Provincias = modelUsuario.ConsultarProvincias();

                // Verificar si el usuario tiene una dirección
                if (datos.ID_Direccion != 0)
                {
                    int idProvinciaSeleccionada = datos.ID_Provincia;
                    int idCantonSeleccionado = datos.ID_Canton;
                    int idDistritoSeleccionado = datos.ID_Distrito;

                    // Modificar la lista de provincias para establecer la seleccionada
                    ViewBag.Provincias = ((IEnumerable<SelectListItem>)ViewBag.Provincias)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Value,
                            Text = p.Text,
                            Selected = (p.Value == idProvinciaSeleccionada.ToString())
                        }).ToList();

                    // Consultar los cantones de la provincia seleccionada
                    var cantones = modelUsuario.cargarCantones(idProvinciaSeleccionada);
                    ViewBag.Cantones = cantones
                        .Select(c => new SelectListItem
                        {
                            Value = c.Value,
                            Text = c.Text,
                            Selected = (c.Value == idCantonSeleccionado.ToString())
                        }).ToList();

                    // Consultar los distritos del cantón seleccionado
                    var distritos = modelUsuario.cargarDistritos(idCantonSeleccionado);
                    ViewBag.Distritos = distritos
                        .Select(d => new SelectListItem
                        {
                            Value = d.Value,
                            Text = d.Text,
                            Selected = (d.Value == idDistritoSeleccionado.ToString())
                        }).ToList();
                }
                else
                {
                    // Si no tiene dirección, establecer los cantones y distritos como cadena vacía
                    ViewBag.Cantones = "";
                    ViewBag.Distritos = "";
                }

                return View(datos);
            }
            catch (Exception ex)
            {
                // Si salta esta excepcion es porque un usuario esta intentando ingresar por url entonces lo envia a la vista de no acceso
                return RedirectToAction("NoAcceso", "Seguridad");
            }
        }



        //Actualiza los datos del cliente
        [AuthorizeRol(1)]
        [HttpPost]
        public ActionResult PerfilAdmin(UsuarioEnt entidad)
        {
            ModelState.Remove("Contrasenna_Usuario");
            if (ModelState.IsValid)
            {

                string correoExistente = modelUsuario.ComprobarCorreoExistenteUsuario(entidad);
                if (correoExistente == "Existe")
                {
                    ViewBag.MensajeNoExitoso = "Ese correo está asociado a otra cuenta";
                    long q = long.Parse(Session["ID_Usuario"].ToString());
                    var datos = modelUsuario.ConsultaClienteEspecifico(q);
                    Session["ID_Usuario"] = datos.ID_Usuario;
                    return View(datos);
                }
                else
                {
                    if (entidad.NuevaContrasenna_Usuario == null || entidad.NuevaContrasenna_Usuario.Length == 0)
                    {
                        entidad.NuevaContrasenna_Usuario = entidad.Contrasenna_Usuario;
                    }
                    else
                    {
                        // Encriptar la nueva contraseña antes de guardarla
                        entidad.NuevaContrasenna_Usuario = seguridad.Encrypt(entidad.NuevaContrasenna_Usuario);
                    }

                    if (entidad.Apellido_Usuario == null || entidad.Apellido_Usuario.Length == 0)
                    {
                        entidad.Apellido_Usuario = string.Empty;
                    }

                    string respuesta = modelUsuario.ActualizarCuentaCliente(entidad);

                    if (respuesta == "OK")
                    {
                        Session["Direccion_Cliente"] = entidad.Direccion_Exacta;

                        return RedirectToAction("PerfilAdmin", "Usuario");

                    }
                    else
                    {
                        ViewBag.MensajeNoExitoso = "No se ha podido actualizar su información";
                        return View();
                    }

                }
            }
            else
            {
                if (entidad.ID_Direccion != 0)
                {
                    var datos = modelUsuario.ConsultaClienteEspecifico(entidad.ID_Usuario);

                    ViewBag.Provincias = modelUsuario.ConsultarProvincias();

                    if (datos.ID_Direccion != 0)
                    {
                        int idProvinciaSeleccionada = datos.ID_Provincia;
                        int idCantonSeleccionado = datos.ID_Canton;
                        int idDistritoSeleccionado = datos.ID_Distrito;

                        ViewBag.Provincias = ((IEnumerable<SelectListItem>)ViewBag.Provincias)
                            .Select(p => new SelectListItem
                            {
                                Value = p.Value,
                                Text = p.Text,
                                Selected = (p.Value == idProvinciaSeleccionada.ToString())
                            }).ToList();

                        var cantones = modelUsuario.cargarCantones(idProvinciaSeleccionada);
                        ViewBag.Cantones = cantones
                            .Select(c => new SelectListItem
                            {
                                Value = c.Value,
                                Text = c.Text,
                                Selected = (c.Value == idCantonSeleccionado.ToString())
                            }).ToList();

                        var distritos = modelUsuario.cargarDistritos(idCantonSeleccionado);
                        ViewBag.Distritos = distritos
                            .Select(d => new SelectListItem
                            {
                                Value = d.Value,
                                Text = d.Text,
                                Selected = (d.Value == idDistritoSeleccionado.ToString())
                            }).ToList();
                        return View(entidad);
                    }
                }


                ViewBag.Provincias = modelUsuario.ConsultarProvincias();
                int idProvinciaS = entidad.ID_Provincia;
                int idCantonS = entidad.ID_Canton;
                int idDistritoS = entidad.ID_Distrito;

                ViewBag.Provincias = ((IEnumerable<SelectListItem>)ViewBag.Provincias)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Value,
                        Text = p.Text,
                        Selected = (p.Value == idProvinciaS.ToString())
                    }).ToList();

                var cantones2 = modelUsuario.cargarCantones(idProvinciaS);
                ViewBag.Cantones = cantones2
                    .Select(c => new SelectListItem
                    {
                        Value = c.Value,
                        Text = c.Text,
                        Selected = (c.Value == idCantonS.ToString())
                    }).ToList();

                var distritos2 = modelUsuario.cargarDistritos(idCantonS);
                ViewBag.Distritos = distritos2
                    .Select(d => new SelectListItem
                    {
                        Value = d.Value,
                        Text = d.Text,
                        Selected = (d.Value == idDistritoS.ToString())
                    }).ToList();
                return View(entidad);
            }

        }
    }
}
