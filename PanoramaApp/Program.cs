using System.Drawing;
#region Linear Stiching

//var img1 = new Bitmap(@"D:\Panomera Test\Frame\4\scene00001.png");
//var img2 = new Bitmap(@"D:\Panomera Test\Frame\4\scene00002.png");

//Bitmap ouput = new PanoramaFunctions.Image().Create(img1, img2);


//for (int i = 2; i <= 133; i+=1)
//{
//    Console.WriteLine($"In => {i}");
//    var path = string.Empty;
//    if (i <= 9)
//    {
//        path = $"D:\\Panomera Test\\Frame\\1\\scene0000{i}.png";

//    }
//    else if (i <= 99)
//    {
//        path = $"D:\\Panomera Test\\Frame\\1\\scene000{i}.png";

//    }
//    else
//    {
//        path = $"D:\\Panomera Test\\Frame\\1\\scene00{i}.png";

//    }
//    if (File.Exists(path))
//    {

//        var img = new Bitmap(path);
//        ouput = new PanoramaFunctions.Image().Create(ouput, img);
//    }

//}


//ouput.Save(@"linear.jpg");
#endregion

#region Binary Stiching

var list = new List<Bitmap>();
var Stage = new List<Bitmap>();
var ImgDirectory = Directory.GetFiles(@"C:\Users\Hussein\Desktop\New folder (2)");
//var ImgDirectory = Directory.GetFiles(@"C:\Users\Hussein\Desktop\Panorama\Sources\Backup\Panorama\Resources\1");



Console.WriteLine("Start...");


foreach (var item in ImgDirectory)
{
    list.Add(new Bitmap(item));
}

while (list.Count() != 1)
{
    if (list.Count() % 2 != 0)
    {
        list[list.Count() - 2] = new PanoramaFunctions.Image()
                                    .Create(list.ElementAt(list.Count() - 2), list.ElementAt(list.Count() - 1));
        list.RemoveAt(list.Count() - 1);
    }


    Console.WriteLine(list.Count());

    for (int i = 0; i < list.Count(); i+=2)
    {
        Stage.Add(new PanoramaFunctions.Image().Create(list.ElementAt(i), list.ElementAt(i+1)));
    }

    list = Stage;
    Stage = new List<Bitmap>();
}

list.First().Save(@"binary.jpg");
#endregion
