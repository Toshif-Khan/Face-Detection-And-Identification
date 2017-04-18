using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using WPFCSharpWebCam;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IFaceServiceClient faceServiceClient = new FaceServiceClient("404062c4115f475cbf6bbd574ed35001");


        
        public MainWindow()
        {
            InitializeComponent();
            webcam = new WebCam();
             webcam.InitializeWebCam(ref captureImage);
        }

        string personGroupId = "meredost";
        WebCam webcam;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create an empty person group
            // string personGroupId = "ncr13";
            // await faceServiceClient.CreatePersonGroupAsync(personGroupId, "");
            bool groupExists = false;
            try
            {
                //MainWindow.Log("Request: Group {0} will be used to build a person database. Checking whether the group exists.", GroupName);
                Title = String.Format("Request: Group {0} will be used to build a person database. Checking whether the group exists.", personGroupId);
                await faceServiceClient.GetPersonGroupAsync(personGroupId);
                groupExists = true;
                //MainWindow.Log("Response: Group {0} exists.", GroupName);
                Title = String.Format("Response: Group {0} exists.", personGroupId);

            }
            catch (FaceAPIException ex)
            {
                if (ex.ErrorCode != "PersonGroupNotFound")
                {
                    //MainWindow.Log("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                    Title = String.Format("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                    return;
                }
                else
                {
                    // MainWindow.Log("Response: Group {0} did not exist previously.", GroupName);
                    Title = String.Format("Response: Group {0} did not exist previously.", personGroupId);
                }
            }

            if (groupExists)
            {
                var cleanGroup = System.Windows.MessageBox.Show(string.Format("Requires a clean up for group \"{0}\" before setting up a new person database. Click OK to proceed, group \"{0}\" will be cleared.", personGroupId), "Warning", MessageBoxButton.OKCancel);
                if (cleanGroup == MessageBoxResult.OK)
                {
                    await faceServiceClient.DeletePersonGroupAsync(personGroupId);
                }
                else
                {
                    return;
                }
            }

            //MainWindow.Log("Request: Creating group \"{0}\"", GroupName);

            Title = String.Format("Request: Creating group \"{0}\"", personGroupId);
            try
            {
                await faceServiceClient.CreatePersonGroupAsync(personGroupId, personGroupId);
                // MainWindow.Log("Response: Success. Group \"{0}\" created", personGroupId);
                Title = String.Format("Response: Success. Group \"{0}\" created", personGroupId);
            }
            catch (FaceAPIException ex)
            {
                //MainWindow.Log("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                Title = String.Format("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                return;
            }


            // Define Anna
            CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Anna"
            );

            // Define Toshif
            CreatePersonResult friend2 = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Toshif"
            );

            // Define Clare
            CreatePersonResult friend3 = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Clare"
            );

            /*// Define Toshif
            CreatePersonResult friend4 = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Toshif"
            );
            */


            // Directory contains image files of Anna
            const string friend1ImageDir = @"D:\Pictures\MyBuddies\Anna\";

            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend1.PersonId, s);
                }
            }

            const string friend2ImageDir = @"D:\Pictures\MyBuddies\Toshif\";

            foreach (string imagePath in Directory.GetFiles(friend2ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Bill
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend2.PersonId, s);
                }
            }

            const string friend3ImageDir = @"D:\Pictures\MyBuddies\Clare\";

            foreach (string imagePath in Directory.GetFiles(friend3ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Clare
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend3.PersonId, s);
                }
            }

            /*const string friend4ImageDir = @"D:\Pictures\MyBuddies\Toshif\";

            foreach (string imagePath in Directory.GetFiles(friend4ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Toshif
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend4.PersonId, s);
                }
            }
            */

            Title = String.Format("Success...Group Scaning Completed.");

        }

        /*private async void Train(string personGroupId)
        {
            await faceServiceClient.TrainPersonGroupAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status.ToString() != "running")
                {
                    break;
                }

                await Task.Delay(1000);
            }

            string testImageFile = @"D:\Pictures\detection2.jpg";

            using (Stream s = File.OpenRead(testImageFile))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();

                var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                foreach (var identifyResult in results)
                {
                    Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                    if (identifyResult.Candidates.Length == 0)
                    {
                        Console.WriteLine("No one identified");
                    }
                    else
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                        Console.WriteLine("Identified as {0}", person.Name);
                    }
                }
            }


        }
        */

        private async void button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var openDlg = new Microsoft.Win32.OpenFileDialog();

            openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";
            bool? result = openDlg.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            string filePath = openDlg.FileName;

            Uri fileUri = new Uri(filePath);
            BitmapImage bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            FacePhoto.Source = bitmapSource;

            Title = "Detecting...";
            FaceRectangle[] faceRects = await UploadAndDetectFaces(filePath);
            Title = String.Format("Detection Finished. {0} face(s) detected", faceRects.Length);

            if (faceRects.Length > 0)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;

                foreach (var faceRect in faceRects)
                {
                    drawingContext.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(Brushes.Red, 2),
                        new Rect(
                            faceRect.Left * resizeFactor,
                            faceRect.Top * resizeFactor,
                            faceRect.Width * resizeFactor,
                            faceRect.Height * resizeFactor
                            )
                    );
                }

                drawingContext.Close();
                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);

                faceWithRectBitmap.Render(visual);
                FacePhoto.Source = faceWithRectBitmap;
            }
            

        }

        private async Task<FaceRectangle[]> UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    return faceRects.ToArray();
                }
            }
            catch (Exception)
            {
                return new FaceRectangle[0];
            }
        }


       

        
       /* private void button1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            webcam.Start();
        }
        */
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            webcam.Start();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FacePhoto.Source = captureImage.Source;
            Helper.SaveImageCapture((BitmapSource)FacePhoto.Source);

            string getDirectory = Directory.GetCurrentDirectory();
            string filename = getDirectory + "\\test1.jpg";
            string filePath = filename;

            Uri fileUri = new Uri(filePath);
            BitmapImage bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            FacePhoto.Source = bitmapSource;

            Title = "Detecting...";
            FaceRectangle[] faceRects = await UploadAndDetectFaces(filePath);
            Title = String.Format("Detection Finished. {0} face(s) detected", faceRects.Length);

            if (faceRects.Length > 0)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;

                foreach (var faceRect in faceRects)
                {
                    drawingContext.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(Brushes.Red, 2),
                        new Rect(
                            faceRect.Left * resizeFactor,
                            faceRect.Top * resizeFactor,
                            faceRect.Width * resizeFactor,
                            faceRect.Height * resizeFactor
                            )
                    );
                }

                drawingContext.Close();
                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);

                faceWithRectBitmap.Render(visual);
                FacePhoto.Source = faceWithRectBitmap;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Helper.SaveImageCapture((BitmapSource)captureImage.Source);
            
            try
            {
                Title = String.Format("Request: Training group \"{0}\"", personGroupId);
                await faceServiceClient.TrainPersonGroupAsync(personGroupId);

                TrainingStatus trainingStatus = null;
                while (true)
                {
                    await Task.Delay(1000);
                    trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);
                    Title = String.Format("Response: {0}. Group \"{1}\" training process is {2}", "Success", personGroupId, trainingStatus.Status);
                    if (trainingStatus.Status.ToString() != "running")
                    {
                        break;
                    }


                }
            }
            catch (FaceAPIException ex)
            {
                //MainWindow.Log("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                Title = String.Format("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
            }

            Title = "Identifing....";
            //string testImageFile = @"D:\Pictures\detection2.jpg";
            string getDirectory = Directory.GetCurrentDirectory();
            string testImageFile = getDirectory + "\\test1.jpg";
            using (Stream s = File.OpenRead(testImageFile))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();
                try
                {
                    var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);

                    foreach (var identifyResult in results)
                    {
                        //Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                        Title = String.Format("Result of face: {0}", identifyResult.FaceId);

                        if (identifyResult.Candidates.Length == 0)
                        {
                            //Console.WriteLine("No one identified");
                            Title = String.Format("No one identified");
                        }
                        else
                        {
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            //Console.WriteLine("Identified as {0}", person.Name);
                            Title = String.Format("Identified as {0}", person.Name);
                        }
                    }
                }
                catch (FaceAPIException ex)
                {
                    return;
                }
            }

        }

        
    }
}