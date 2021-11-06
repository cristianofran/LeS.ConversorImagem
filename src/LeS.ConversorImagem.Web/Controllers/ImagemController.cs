using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeS.ConversorImagem.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagemController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        public string Pasta => $"{_environment.WebRootPath}\\imagens\\";

        public ImagemController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var images = new List<object>();

            foreach (var file in Directory.GetFiles(Pasta))
            {
                var fi = new FileInfo(file);
                images.Add(new
                {
                    Extension = fi.Extension,
                    Length = fi.Length,
                    Name = fi.Name,
                    OriginalPath = file
                });
            }

            return Ok(images);
        }

        [HttpGet("{nome}")]
        public async Task<ActionResult> Get(string nome)
        {
            var caminho = Directory.GetFiles(Pasta).FirstOrDefault(x => x.Contains(nome));
            var bytes = await System.IO.File.ReadAllBytesAsync(caminho);
            return File(bytes, "application/octet-stream", Path.GetFileName(caminho));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> EnviaArquivo([FromForm] IFormFile arquivo)
        {
            
            if (arquivo.Length > 0)
            {
                if (!Directory.Exists(Pasta))
                {
                    Directory.CreateDirectory(Pasta);
                }

                var destino = $"{Pasta}{arquivo.FileName}";
                using (var stream = new FileStream(destino, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                    stream.Flush();
                    return Ok(new
                    {
                        ContentType = arquivo.ContentType,
                        FileName = arquivo.FileName,
                        Length = arquivo.Length
                    });
                }
                /*using (var filestream = System.IO.File.Create(destino))
                {
                    await arquivo.CopyToAsync(filestream);
                    filestream.Flush();
                    return Ok(new
                    {
                        ContentType = arquivo.ContentType,
                        FileName = arquivo.FileName,
                        Length = arquivo.Length
                    });
                }*/
            }
            return BadRequest();
        }
    }
}