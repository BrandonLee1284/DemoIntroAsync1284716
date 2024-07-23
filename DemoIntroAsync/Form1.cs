using System.Diagnostics;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directrioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directrioActual, @"Imagenes\resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directrioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecución(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("Inicio");
            List<Imagenes> imagenes = ObtenerImagenes();

            //Parte Secuencial 

            var sw = new  Stopwatch();

            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }

            Console.WriteLine("Seguencial - duracion en seguntod: {0}",
                    sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();

            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);

            Console.WriteLine("Paralelo - duración en segundos: {0}",
                    sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();

          
            pictureBox1.Visible = false;
        }

        private async Task ProcesarImagen(string directorio, Imagenes imagenes)
        {
            var respuesta = await httpClient.GetAsync(imagenes.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagenes.Nombre);
            bitmap.Save(destino);
        }
        private static List<Imagenes> ObtenerImagenes()
        {
            var imagenes = new List<Imagenes>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $"Mono Capuchino {i}.png,",
                        URL = "https://th.bing.com/th/id/OIP.g2kx9VNRKNG4bnei4S1W6wHaEK?rs=1&pid=ImgDetMain"
                    });

                imagenes.Add(
                     new Imagenes()
                     {
                        Nombre = $"Mono Capuchino {i}.png,",
                        URL ="https://www.bing.com/images/search?view=detailV2&ccid=RssdVryK&id=8D1B11AFBAFFFD92E9EA84173A47B527A049E87A&thid=OIP.RssdVryK4Nxp5Nia5gMkwAAAAA&mediaurl=https%3a%2f%2fi.pinimg.com%2f736x%2f41%2fb6%2f6e%2f41b66e1ef1f87c8623417771814b535d.jpg&cdnurl=https%3a%2f%2fth.bing.com%2fth%2fid%2fR.46cb1d56bc8ae0dc69e4d89ae60324c0%3frik%3deuhJoCe1RzoXhA%26pid%3dImgRaw%26r%3d0&exph=355&expw=474&q=monos+capuchinos+memes&simid=607998689367509945&FORM=IRPRST&ck=60B99C981AB708BA5907AC648D68049E&selectedIndex=9&itb=0&ajaxhist=0&ajaxserp=0"
                     });

                imagenes.Add(
                    new Imagenes()
                    {
                        Nombre =$"Mono Capuchino {i}.png,",
                        URL = "https://media.metrolatam.com/2018/04/11/1231e42qrd-4deecf92816ee6a1511e0d3c10ceca13-1200x600.jpg"
                    });
            }

            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecución(string destinoBaseParalelo, string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            } 
            
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo);
        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(3000);// asíncrono
            return "Gabriel";
        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);// asíncrono
            Console.WriteLine("Proceso A finalizado");
        }   
        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);// asíncrono
            Console.WriteLine("Proceso B finalizado");
        }  
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);// asíncrono
            Console.WriteLine("Proceso C finalizado");
        }


    }
}
