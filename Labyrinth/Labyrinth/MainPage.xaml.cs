using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

#if WINDOWS
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using WinRT.Interop;
#endif

namespace Labyrinth
{
    public partial class MainPage : ContentPage
    {
        private List<List<Button>> Maze = new();
        private int MazeHeight;
        private int MazeWidth;

        public MainPage()
        {
            InitializeComponent();
        }

        public void Build(object sender, System.EventArgs e)
        {
            try
            {
                if (height.Text == null || width.Text == null)
                {
                    throw new Exception();
                }
                MazeHeight = Convert.ToInt16(height.Text);
                MazeWidth = Convert.ToInt16(width.Text);
                if (MazeHeight <= 0 || MazeWidth <= 0)
                {
                    throw new Exception();
                }

                List<List<int>> MazeInt = new();

                for (int i = 0; i < MazeWidth; i++)
                {
                    MazeInt.Add(new List<int>());
                    for (int j = 0; j < MazeHeight; j++)
                    {
                        MazeInt[i].Add(0);
                    }
                }

                create(MazeInt);

                input.IsVisible = false;
                maze.IsVisible = true;
            }
            catch
            {
                error.IsVisible = true;
            }
        }
#if WINDOWS
        public async void save(object sender, EventArgs e)
        {
            List<List<int>> MazeInt = new();

            for (int i = 0; i < MazeWidth; i++)
            {
                MazeInt.Add(new List<int>());
                for (int j = 0; j < MazeHeight; j++)
                {
                    Button bt = Maze[i][j];
                    if (bt.BackgroundColor.Equals(new Color(0, 0, 0)))
                    {
                        MazeInt[i].Add(0);
                    }
                    else if (bt.BackgroundColor.Equals(new Color(255, 0, 0)))
                    {
                        MazeInt[i].Add(1);
                    }
                    else
                    {
                        MazeInt[i].Add(2);
                    }
                }
            }

            var json = JsonSerializer.Serialize(MazeInt);

            FileSavePicker picker = new();
            picker.FileTypeChoices.Add("JSON", new List<string>() { ".json" });
            picker.SuggestedFileName = "maze";
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            InitializeWithWindow.Initialize(picker, System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);

            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                await FileIO.WriteTextAsync(file, json);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        public async void load(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync();

            List<List<int>> MazeInt = JsonSerializer.Deserialize<List<List<int>>>(new StreamReader(File.OpenRead(result.FullPath)).ReadToEnd());

            create(MazeInt);
        }
#endif

        private void create(List<List<int>> MazeInt)
        {
            Maze = new List<List<Button>>();

            MazeWidth = MazeInt.Count;
            MazeHeight = MazeInt[0].Count;

            grid.Clear();

            for (int i = 0; i < MazeWidth; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            }
            for (int i = 0; i < MazeHeight; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            for (int i = 0; i < MazeWidth; i++)
            {
                Maze.Add(new List<Button>());
                for (int j = 0; j < MazeHeight; j++)
                {
                    Maze[i].Add(new Button());

                    switch (MazeInt[i][j])
                    {
                        case 0:
                            Maze[i][j].BackgroundColor = new Color(0, 0, 0);
                            break;
                        case 1:
                            Maze[i][j].BackgroundColor = new Color(255, 0, 0);
                            break;
                        case 2:
                            Maze[i][j].BackgroundColor = new Color(0, 0, 255);
                            break;
                    }

                    Maze[i][j].Clicked += (sender, e) =>
                    {
                        Button bt = (Button)sender;
                        if (bt.BackgroundColor.Equals(new Color(0, 0, 0)))
                        {
                            bt.BackgroundColor = new Color(255, 0, 0);

                        }
                        else if (bt.BackgroundColor.Equals(new Color(255, 0, 0)))
                        {
                            bt.BackgroundColor = new Color(0, 0, 255);
                        }
                        else
                        {
                            bt.BackgroundColor = new Color(0, 0, 0);
                        }
                    };

                    grid.Add(Maze[i][j], i, j);
                }
            }
        }
    }
}