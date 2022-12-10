using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TestUploader;

/// <summary>
/// Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog();
        if (ofd.ShowDialog() ?? false)
        {
            fileName.Text = ofd.FileName;

            //var utf8 = Encoding.UTF8;
            //var utf16 = Encoding.Unicode;
            //var bytes16 = utf16.GetBytes(ofd.FileName);
            //var bytes8 = Encoding.Convert(utf16, utf8, bytes16);
            //string s = utf8.GetString(bytes8);

            //using var sw = new StreamWriter("test.txt", false, Encoding.ASCII);
            //sw.Write(utf8.GetString(bytes8));
            //sw.Close();
        }
        //Sample.Run();
    }

    private async void Button_Click_1(object sender, RoutedEventArgs e)
    {
		//Task result = Upload(fileName.Text, "http://localhost:13542/api/FileStore/Upload");
		//Task result = Upload(fileName.Text, "http://localhost:13542/File/Upload");

		if (sender is not Button button)
		{
            return;
		}

		button.IsEnabled = false;
        try
        {
            await UploadAsync(fileName.Text, "http://localhost:8085/UVAFileStore/File/Upload");
            MessageBox.Show("Файл загружен!");
        }
        catch (Exception ex)
        {
			MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
        finally
        {
            button.IsEnabled = true;
        }
    }

    public static async Task UploadAsync(string filePath, string uri)
    {
        var finfo = new FileInfo(filePath);
		using var fs = new FileStream(filePath, FileMode.Open);
		using var ms = new MemoryStream();
		await fs.CopyToAsync(ms).ConfigureAwait(false);

		byte[] data = ms.ToArray();

		var handler = new HttpClientHandler()
		{
			UseDefaultCredentials = true
		};

		using var client = new HttpClient(handler);
		using var content = new MultipartFormDataContent();
		var streamContent = new StreamContent(new MemoryStream(data));
		string mimeType = GetMimeType(finfo.Name);
		streamContent.Headers.Add("Content-Type", mimeType);
		streamContent.Headers.Add("Content-Disposition", "form-data; name=\"uploadFile\"; filename=\"" + finfo.Name + "\"");
		content.Add(streamContent, "uploadFile", finfo.Name);

		string requestUri = uri + "?fileName=" + finfo.Name + "&Desc=" + finfo.FullName;
		var result = await client.PostAsync(requestUri, content).ConfigureAwait(false);
		if (result.IsSuccessStatusCode)
		{
			MessageBox.Show(result.StatusCode.ToString() + ". " + result.RequestMessage.ToString(), "Успешно!");
		}
		else
		{
			MessageBox.Show(result.StatusCode.ToString() + ". " + result.RequestMessage.ToString(), "Неудачно!");
		}
	}

    public static string GetMimeType(string fileName)
    {
        string mimeType = "application/unknown";
        string ext = System.IO.Path.GetExtension(fileName).ToLower();
			RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext); // henter info fra windows registry

        if (regKey != null && regKey.GetValue("Content Type") != null)
        {
            mimeType = regKey.GetValue("Content Type").ToString();
        }
        else if (ext == ".png") // a couple of extra info, due to missing information on the server
        {
            mimeType = "image/png";
        }
        else if (ext == ".flv")
        {
            mimeType = "video/x-flv";
        }

        return mimeType;
    }
}
