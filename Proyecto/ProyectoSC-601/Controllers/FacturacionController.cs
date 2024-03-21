﻿using ProyectoSC_601.Models;
using System.Web.Mvc;

namespace ProyectoSC_601.Controllers
{
    public class FacturacionController : Controller
    {

        FacturacionModel modelFacturacion = new FacturacionModel();
        IndexModel modelIndex = new IndexModel();

        //Historial de Compras Cliente

        [HttpGet]
        public ActionResult FacturacionCliente()
        {
            if (Session["ID_Usuario"] != null && Session["Rol"] != null && long.Parse(Session["Rol"].ToString()) == 2)
            {
                // Obtiene la cantidad de productos diferentes en el carrito
                int cantidadProductos = modelIndex.ObtenerCantidadProductosEnCarrito(long.Parse(Session["ID_Usuario"].ToString()));

                // Pasa la cantidad de productos a la vista
                ViewBag.CantidadProductosEnCarrito = cantidadProductos;
            }
            var datos = modelFacturacion.ConsultaFacturasCliente(long.Parse(Session["ID_Usuario"].ToString()));
            return View(datos);
        }

        [HttpGet]
        public ActionResult FacturaDetalleCliente(long q)
        {
            if (Session["ID_Usuario"] != null && Session["Rol"] != null && long.Parse(Session["Rol"].ToString()) == 2)
            {
                // Obtiene la cantidad de productos diferentes en el carrito
                int cantidadProductos = modelIndex.ObtenerCantidadProductosEnCarrito(long.Parse(Session["ID_Usuario"].ToString()));

                // Pasa la cantidad de productos a la vista
                ViewBag.CantidadProductosEnCarrito = cantidadProductos;
            }
            var datos = modelFacturacion.ConsultaDetalleFactura(q);
            return View(datos);
        }


        // Consulta Facturas Administrador 

        [HttpGet]
        public ActionResult Facturacion()
        {
            var datos = modelFacturacion.ConsultaFacturasAdmin();
            return View(datos);
        }

        [HttpGet]
        public ActionResult FacturaDetalle(long q)
        {
            var datos = modelFacturacion.ConsultaDetalleFactura(q);
            return View(datos);
        }

    }
}