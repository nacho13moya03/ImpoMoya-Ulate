﻿using ProyectoSC_601.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Web.Mvc;

namespace ProyectoSC_601.Models
{
    public class ProveedorModel
    {
        public string rutaServidor { get; } = ((NameValueCollection)ConfigurationManager.GetSection("secureAppSettings"))["RutaApi"];
        public string CredentialsSmarter { get; } = ((NameValueCollection)ConfigurationManager.GetSection("secureAppSettings"))["Credentials"];
        public string HeaderlsSmarter { get; } = ((NameValueCollection)ConfigurationManager.GetSection("secureAppSettings"))["AuthorizationHeader"];

        public List<SelectListItem> ConsultarIdentificacionesProveedor()
        {
            using (var client = new HttpClient())
            {
                var credentials = CredentialsSmarter;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var urlApi = rutaServidor + "ConsultarIdentificacionesProveedor";
                var res = client.GetAsync(urlApi).Result;
                return res.Content.ReadFromJsonAsync<List<SelectListItem>>().Result;
            }
        }

        public string ComprobarCedulaProveedor(ProveedorEnt entidad)
        {
            using (var client = new HttpClient())
            {
                var credentials = CredentialsSmarter;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var urlApi = rutaServidor + "ComprobarCedulaProveedor";
                var jsonData = JsonContent.Create(entidad);
                var res = client.PostAsync(urlApi, jsonData).Result;
                return res.Content.ReadFromJsonAsync<string>().Result;
            }
        }

        /*Esto envía una solicitud HTTP POST a una API para registrar un proveedor, 
        convierte la respuesta a una cadena y la devuelve. */
        public string RegistrarProveedor(ProveedorEnt entidad)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = CredentialsSmarter;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var urlApi = rutaServidor + "RegistrarProveedor";
                    var jsonData = JsonContent.Create(entidad);
                    var res = client.PostAsync(urlApi, jsonData).Result;
                    return res.Content.ReadFromJsonAsync<string>().Result;
                }
            }
            catch (Exception ex)
            {
                return "Error al intentar comunicarse con el servidor: " + ex.Message;
            }
        }



        /*Es una solicitud HTTP GET a una API para obtener la lista de empresas como objetos 
        SelectListItem de las empresas disponibles*/
        public List<SelectListItem> ConsultarEmpresas()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = CredentialsSmarter;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var urlApi = rutaServidor + "ConsultarEmpresas";
                    var res = client.GetAsync(urlApi).Result;
                    return res.Content.ReadFromJsonAsync<List<SelectListItem>>().Result;
                }
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
        }



        /*Es una solicitud HTTP GET a una API para obtener la lista de todos los proveedores como objetos ProveedorEnt*/
        public List<ProveedorEnt> ConsultaProveedores()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = CredentialsSmarter;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var urlApi = rutaServidor + "ConsultaProveedores";
                    var res = client.GetAsync(urlApi).Result;

                    return res.Content.ReadFromJsonAsync<List<ProveedorEnt>>().Result;
                }
            }
            catch (Exception)
            {
                return new List<ProveedorEnt>();
            }
        }



        /*Es una solicitud HTTP PUT a una API para actualizar el estado de un proveedor*/
        public string ActualizarEstadoProveedor(ProveedorEnt entidad)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = CredentialsSmarter;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var urlApi = rutaServidor + "ActualizarEstadoProveedor";
                    var jsonData = JsonContent.Create(entidad);
                    var res = client.PutAsync(urlApi, jsonData).Result;
                    return res.Content.ReadFromJsonAsync<string>().Result;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



        /*Este código realiza una solicitud HTTP PUT a una API para actualizar la información completa de un proveedor*/
        public string ActualizarProveedor(ProveedorEnt entidad)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = CredentialsSmarter;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var urlApi = rutaServidor + "ActualizarProveedor";
                    var jsonData = JsonContent.Create(entidad);
                    var res = client.PutAsync(urlApi, jsonData).Result;
                    return res.Content.ReadFromJsonAsync<string>().Result;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



        /*Esto sirve para hacer una solicitud HTTP GET a una API para consultar la información de un proveedor específico. 
        El parámetro q se utiliza para identificar al proveedor y utilizar la variable q para mantener la privacidad de los nombres*/
        public ProveedorEnt ConsultaProveedor(long q)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = CredentialsSmarter;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var urlApi = rutaServidor + "ConsultaProveedor?q=" + q;
                    var res = client.GetAsync(urlApi).Result;
                    return res.Content.ReadFromJsonAsync<ProveedorEnt>().Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }



        /*Esta parte del código realiza una solicitud HTTP DELETE a una API para eliminar un proveedor. 
        Utiliza el ID del proveedor en la URL de la solicitud como en el actualizar y utilizar la variable q para mantener la privacidad de los nombres*/
        public string EliminarProveedor(long idProveedor)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = CredentialsSmarter;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var urlApi = rutaServidor + $"EliminarProveedor?q={idProveedor}";
                    var res = client.DeleteAsync(urlApi).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        var respuestaJson = res.Content.ReadAsStringAsync().Result;

                        if (respuestaJson.Contains("OK"))
                        {
                            return "OK";
                        }
                        else
                        {
                            return "Error en la respuesta del servicio.";
                        }
                    }
                    else
                    {
                        return "Error en la solicitud al servicio.";
                    }
                }
            }
            catch (Exception)
            {
                return "Error en la ejecución del servicio.";
            }
        }





    }
}