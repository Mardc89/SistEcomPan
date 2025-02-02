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
    public class ProductoService:IProductoService
    {
        private readonly IGenericRepository<Productos> _repositorio;
        
        public ProductoService(IGenericRepository<Productos> repositorio, IHostEnvironment environment)
        {
            _repositorio = repositorio;
        }

        public Task<string> ConsultarProducto()
        {
            throw new NotImplementedException();
        }

        public async Task<Productos> Crear(Productos entidad, Stream Foto = null, string NombreFoto = "")
        {

            Productos productoExiste = await _repositorio.Buscar(entidad.Descripcion,null,null);

            if (productoExiste != null)
                throw new TaskCanceledException("El Producto ya Existe");

            try
            {
                entidad.NombreImagen = NombreFoto;

                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagenesProducto");
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

            Productos productoExiste = await _repositorio.Verificar(entidad.Descripcion,null,entidad.IdProducto);

            if (productoExiste != null)
                throw new TaskCanceledException("El Producto ya Existe");

            try
            {

                Productos productoEditar = await _repositorio.Buscar(null, null, entidad.IdProducto);

                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.IdCategoria = entidad.IdCategoria;
                productoEditar.Precio = entidad.Precio;
                productoEditar.Estado = entidad.Estado;
                productoEditar.Stock = entidad.Stock;


                if (productoEditar.NombreImagen == "" || NombreFoto!="")
                    productoEditar.NombreImagen = NombreFoto;
                
                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagenesProducto");
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

                Productos productoEncontrado = await _repositorio.Buscar(null,null,IdProducto);

                if (productoEncontrado == null)
                    throw new TaskCanceledException("El Producto no Existe");

                string nombreFoto = productoEncontrado.NombreImagen;
                bool respuesta = await _repositorio.Eliminar(productoEncontrado.IdProducto);

                if (productoEncontrado.UrlImagen!="")
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

        public async Task<IQueryable<Productos>> ObtenerNombre()
        {
            List<Productos> lista = await _repositorio.Lista();
            return lista.AsQueryable();
        }






    }
}
