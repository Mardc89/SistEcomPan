using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Hosting;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{
    public class ProductoService
    {
        private readonly IGenericRepository<Productos> _repositorio;
        private readonly IHostEnvironment _environment;
        public ProductoService(IGenericRepository<Productos> repositorio, IHostEnvironment environment)
        {
            _repositorio = repositorio;
            _environment = environment;
        }

        public async Task<Productos> Crear(Productos entidad, Stream Foto = null, string NombreFoto = "")
        {

            IQueryable<Productos> productos = await _repositorio.Consultar();
            IQueryable<Productos> productoEvaluado = productos.Where(u => u.Descripcion == entidad.Descripcion);
            Productos productoExiste = productoEvaluado.FirstOrDefault();


            if (productoExiste != null)
                throw new TaskCanceledException("El Producto ya Existe");

            try
            {
                entidad.NombreImagen = NombreFoto;

                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgProducto");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullpath = Path.Combine(path, NombreFoto);

                    string UrlFoto = fullpath;

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        Foto.CopyTo(stream);

                    }
                    entidad.UrlImagen = UrlFoto;
                }

                Productos productoCreado = await _repositorio.Crear(entidad);

                if (productoCreado.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear el Producto");

                return productoCreado;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<Productos> Editar(Productos entidad, Stream Foto = null, string NombreFoto = "")
        {

            IQueryable<Productos> productos = await _repositorio.Consultar();
            IQueryable<Productos> productoEvaluado = productos.Where(u => u.Descripcion == entidad.Descripcion && u.IdProducto != entidad.IdProducto);
            Productos productoExiste = productoEvaluado.FirstOrDefault();


            if (productoExiste != null)
                throw new TaskCanceledException("El Producto ya Existe");

            try
            {
                IQueryable<Productos> buscarProducto = await _repositorio.Consultar();
                IQueryable<Productos>productoEncontrado = buscarProducto.Where(u => u.IdProducto == entidad.IdProducto);
                Productos productoEditar = productoEncontrado.First();

                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.IdCategoria = entidad.IdCategoria;
                productoEditar.Precio= entidad.Precio;
                productoEditar.Estado = entidad.Estado;
                productoEditar.Stock = entidad.Stock;
                

                if (productoEditar.NombreImagen == "")
                    productoEditar.NombreImagen = NombreFoto;

                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgProducto");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullpath = Path.Combine(path, NombreFoto);

                    string UrlFoto = fullpath;

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        Foto.CopyTo(stream);

                    }
                    productoEditar.UrlImagen = UrlFoto;
                }

                bool respuesta = await _repositorio.Editar(productoEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el producto");

                return productoEditar;


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> Eliminar(int IdProducto)
        {
            try
            {
                IQueryable<Productos> productos = await _repositorio.Consultar();
                IQueryable<Productos> productoEvaluado = productos.Where(u => u.IdProducto == IdProducto);
                Productos productoEncontrado = productoEvaluado.FirstOrDefault();

                if (productoEncontrado == null)
                    throw new TaskCanceledException("El Producto no Existe");

                string nombreFoto = productoEncontrado.NombreImagen;
                bool respuesta = await _repositorio.Eliminar(productoEncontrado.IdProducto);

                if (respuesta)
                    System.IO.File.Delete(productoEncontrado.UrlImagen);

                return true;

            }
            catch (Exception)
            {

                throw;
            }
        }



        public async Task<List<Productos>> Lista()
        {
            List<Productos> query = await _repositorio.Lista();
            return query;
        }






    }
}
