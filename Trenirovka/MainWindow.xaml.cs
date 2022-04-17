using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Trenirovka
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User Polzovatel = new User();
        public MainWindow()
        {
            InitializeComponent();
            CheckUser();
        }

        private void FillindDataToBase()
        {
            Trenirovki trenirovki = new Trenirovki()
            {
                Name = "Комплекс упражнений направленные на массу тела",
                TipeIndex = 1
            };
            var setter = client.Set("TrenirovkiList/1/" + trenirovki.Name, trenirovki);
        }
        private void FillTrenirovka()
        {
            Uprazninie a = new Uprazninie("Универсальный комплекс тренировок", true, 1, "Гиперэкстензия", "Физическое упражнение для развития выпрямителей спины, сгибателей голени и ягодичных мышц.",
                10 + GetKollichestov(),
                @"https://sport-i-zdorovie.ru/wp-content/uploads/2019/11/giperjekstenzija-myshcy.jpg",
                "Скрестите руки за головой или на груди. Выпрямите корпус в абсолютно прямую линию.Взгляд строго вперед. Центр тяжести на пятках. Опускайте корпус вниз, сгибаясь только в пояснице. Дойдя до угла 90°, верните корпус назад в исходное положение. При движении не допускайте рывков и переразгибаний спины. Задержитесь в верхней точке на 1-2 секунды");
            var setter = client.Set("TrenirovkiList/2/Upraznenie/" + a.Name, a);

            Uprazninie aФ = new Uprazninie("Универсальный комплекс тренировок", true, 1, "Скручивания на наклонной скамье", "Скручивания на наклонной скамье – это эффективное упражнение для мышц кора, непосредственно прорабатывает прямую мышцу живота.",
              13 + GetKollichestov(),
              @"https://101hairtips.com/wp-content/uploads/c/6/c/c6c7eb20c756b0356e4a555034bec4f8.jpg",
              "Расположите ноги под специальными валиками для фиксации коленных и голеностопных суставов. Бедра и ягодицы должны полностью соприкасаться с наклонной скамьей. Заведите руки за голову или держите на груди – это упростит нагрузку. Округлите спину в верхней точке движения и не разгибайте позвоночник до конца подхода. На вдохе отводите туловище назад, сохраняя напряжение в мышцах живота, и опустите до такой степени, чтобы угол между туловищем и бедрами составлял примерно 90 градусов. Не опускайте спину на скамью, помните о напряжении в мышцах живота и округлении позвоночника. С выдохом усилием пресса скручивайтесь, приводя живот к бедрам, но не выпрямляйте спину.");
            var setterФ = client.Set("TrenirovkiList/2/Upraznenie/" + aФ.Name, aФ);
            
        }

        IFirebaseConfig fcon = new FirebaseConfig()
        {
            AuthSecret = "Ypj2iHSePko2ni02eUFiYafaGjRUy4wliLAuPy1d",
            BasePath = "https://training-5533b-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(fcon);
                //FillindDataToBase();
                //FillTrenirovka();
                FillListBox();
            }
            catch
            {
                MessageBox.Show("Проверьте подключение к интернету");
            }
        }
        public List<Trenirovki> listTrenirovki = new List<Trenirovki>();
        public List<Uprazninie> listUprazniniyaUp = new List<Uprazninie>();
        public List<Uprazninie> listUprazniniyaDown = new List<Uprazninie>();
        public void FillListBox()
        {
            listTrenirovki.Clear();
            MyList.ItemsSource = null;
            FirebaseResponse res = client.Get($"TrenirovkiList/{Polzovatel.TipeIndex}/");
            Dictionary<string,Trenirovki> data = JsonConvert.DeserializeObject<Dictionary<string, Trenirovki>>(res.Body.ToString());
            foreach(var a in data)
            {
                Trenirovki b = new Trenirovki()
                {
                    Name = a.Value.Name,
                    TipeIndex = a.Value.TipeIndex
                };
                if(b.TipeIndex == Polzovatel.TipeIndex)
                  listTrenirovki.Add(b);
            }

            MyList.ItemsSource = listTrenirovki;



            Dostupnie_Treni.Clear();
            MyTrenigovki.ItemsSource = null;
            FirebaseResponse resA = client.Get($"Users/{Polzovatel.ID_User}/Trenirovka/");
            Dictionary<string, LichnieTrenirovki> dataA = JsonConvert.DeserializeObject<Dictionary<string, LichnieTrenirovki>>(resA.Body.ToString());
            foreach (var a in dataA)
            {
                LichnieTrenirovki b = new LichnieTrenirovki()
                {
                    Name = a.Value.Name,
                    ID_Sozdatel = a.Value.ID_Sozdatel,
                    Opisanie = a.Value.Opisanie,
                    Rating = a.Value.Rating,
                    CountOfVotes = a.Value.CountOfVotes,
                    ListOfUprazneniy = a.Value.ListOfUprazneniy
                };
                Dostupnie_Treni.Add(b);
            }

            MyTrenigovki.ItemsSource = Dostupnie_Treni;
            //AllTrenirovki

            Vsse_Treni.Clear();
            
            FirebaseResponse resAB = client.Get($"ExportedTrenirovku/");
            Dictionary<string, LichnieTrenirovki> dataAB = JsonConvert.DeserializeObject<Dictionary<string, LichnieTrenirovki>>(resAB.Body.ToString());
            if (dataAB != null)
            {
                foreach (var a in dataAB)
                {
                    LichnieTrenirovki b = new LichnieTrenirovki()
                    {
                        Name = a.Value.Name,
                        ID_Sozdatel = a.Value.ID_Sozdatel,
                        Opisanie = a.Value.Opisanie,
                        Rating = a.Value.Rating,
                        CountOfVotes = a.Value.CountOfVotes,
                        ListOfUprazneniy = a.Value.ListOfUprazneniy

                    };
                    Vsse_Treni.Add(b);
                }
            }

            AllTrenirovki.ItemsSource = Vsse_Treni;

        }
        public string CurretDir = Directory.GetCurrentDirectory();
        public void CheckUser()
        {
            if(File.Exists($@"{CurretDir}\UserData.dat") == false)
            {
                CheckUser a = new CheckUser();
                a.Top = this.Top;
                a.Left = this.Left;
                this.Hide();
                a.Show();
            }
            else
            {
                using (BinaryReader reader = new BinaryReader(File.Open($@"{CurretDir}\UserData.dat", FileMode.Open)))
                {
                    while (reader.PeekChar() > -1)
                    {
                        Polzovatel.TipeIndex = reader.ReadInt32();
                        Polzovatel.Index = reader.ReadDouble();
                        Polzovatel.Gender = reader.ReadString();
                        Polzovatel.Age = reader.ReadInt32();
                        Polzovatel.Weight = reader.ReadInt32();
                        Polzovatel.Rost = reader.ReadInt32();
                        Polzovatel.ID_User = reader.ReadInt32();
                    }
                }
            }

           
        }

        private void MyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            listUprazniniyaUp.Clear();
            listUprazniniyaDown.Clear();
            UpList.ItemsSource = null;
            DownList.ItemsSource = null;

            FirebaseResponse res = client.Get($"TrenirovkiList/{Polzovatel.TipeIndex}/Upraznenie/");
            Dictionary<string, Uprazninie> data = JsonConvert.DeserializeObject<Dictionary<string, Uprazninie>>(res.Body.ToString());
            if (data != null)
            {
                foreach (var a in data)
                {
                    Uprazninie b = new Uprazninie(a.Value.NameTrenirovki, a.Value.Verhnaya, a.Value.Number, a.Value.Name, a.Value.Opisanie,
                    a.Value.KollischestvoPovtorov,
                   a.Value.Mishchi,
                    a.Value.HowToDoIt);

                    if (b.Verhnaya == true)
                    {
                        if (listTrenirovki[MyList.SelectedIndex].Name == b.NameTrenirovki)
                            listUprazniniyaUp.Add(b);
                    }
                    else
                    {
                        if (listTrenirovki[MyList.SelectedIndex].Name == b.NameTrenirovki)
                            listUprazniniyaDown.Add(b);
                    }
                }
            }


            UpList.ItemsSource = listUprazniniyaUp;
            DownList.ItemsSource = listUprazniniyaDown;
        }
        private int GetKollichestov()
        {
            int resultAdding = 0;
            if (Polzovatel.Age > 18 && Polzovatel.Age < 30)
                resultAdding += 1;
            if (Polzovatel.Age >= 30 && Polzovatel.Age < 50)
                resultAdding += 2;
            if (Polzovatel.Age >= 50 && Polzovatel.Age < 110)
                resultAdding += -1;
            if (Polzovatel.Gender == "М")
                resultAdding += 1;


            return resultAdding;
        }

        private void UpList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MoreData a = new MoreData(listUprazniniyaUp[UpList.SelectedIndex]);
                a.Top = this.Top;
                a.Left = this.Left;
                a.Show();
            }
            catch
            {

            }
        }

        private void DownList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MoreData a = new MoreData(listUprazniniyaDown[DownList.SelectedIndex]);
                a.Top = this.Top;
                a.Left = this.Left;
                a.Show();
            }
            catch
            {

            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        List<LichnieTrenirovki> Dostupnie_Treni = new List<LichnieTrenirovki>();
        List<LichnieTrenirovki> Vsse_Treni = new List<LichnieTrenirovki>();
        List<Uprazninie> Dostupnie_Uprazneniya = new List<Uprazninie>();

        private void MyTrenigovki_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            Dostupnie_Uprazneniya.Clear();
            Trenya.ItemsSource = null;

            FirebaseResponse res = client.Get($"Users/{Polzovatel.ID_User}/Trenirovka/");
            Dictionary<string, LichnieTrenirovki> data = JsonConvert.DeserializeObject<Dictionary<string, LichnieTrenirovki>>(res.Body.ToString());
            if (data != null)
            {
                foreach (var a in data)
                {
                    try
                    {
                        if (a.Value.Name == Dostupnie_Treni[MyTrenigovki.SelectedIndex].Name)
                        {
                            foreach (Uprazninie ABS in a.Value.ListOfUprazneniy)
                            {
                                Dostupnie_Uprazneniya.Add(ABS);
                            }

                        }
                    }
                    catch
                    {

                    }
                }
            }


            Trenya.ItemsSource = Dostupnie_Uprazneniya;
        }

        private void Trenya_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MoreData a = new MoreData(Dostupnie_Uprazneniya[Trenya.SelectedIndex]);
                a.Top = this.Top;
                a.Left = this.Left;
                a.Show();
            }
            catch
            {
                
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MyTrenigovki.SelectedItem != null)
            {
                Adding_Upraznenie a = new Adding_Upraznenie(Polzovatel, Dostupnie_Treni[MyTrenigovki.SelectedIndex].Name);
                a.Top = this.Top;
                a.Left = this.Left;
                a.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Сначала выберете, в какую тренировку будете добавлять упражнение");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string Key = Dostupnie_Uprazneniya[Trenya.SelectedIndex].Name;
            if (Trenya.SelectedItem != null)
            {
                FirebaseResponse res = client.Get($"Users/{Polzovatel.ID_User}/Trenirovka/");
                Dictionary<string, LichnieTrenirovki> data = JsonConvert.DeserializeObject<Dictionary<string, LichnieTrenirovki>>(res.Body.ToString());
                if (data != null)
                {
                    foreach (var a in data)
                    {
                        if (a.Value.Name == Dostupnie_Treni[MyTrenigovki.SelectedIndex].Name)
                        {
                            LichnieTrenirovki abs = new LichnieTrenirovki()
                            {
                                Name = a.Value.Name,
                                Opisanie = a.Value.Opisanie,
                                Rating = a.Value.Rating,
                                ID_Sozdatel = a.Value.ID_Sozdatel,
                                CountOfVotes = a.Value.CountOfVotes,
                                ListOfUprazneniy = a.Value.ListOfUprazneniy
                            };
                                foreach (Uprazninie absc in a.Value.ListOfUprazneniy)
                                {
                                    if (Dostupnie_Uprazneniya[Trenya.SelectedIndex].Name == absc.Name)
                                    {
                                        a.Value.ListOfUprazneniy.Remove(absc);
                                        break;
                                    }
                                }
                            var setter = client.Set($"Users/{Polzovatel.ID_User}/Trenirovka/{Dostupnie_Treni[MyTrenigovki.SelectedIndex].Name}", abs);
                            Trenya.SelectedItem = null;
                            FillListBox();

                        }
                    }
                }
                else
                    MessageBox.Show("Как вы удаляете что-то, если и так ничего нет T_T");
                //var setter = client.Delete($"Users /{ Polzovatel.ID_User}/ Trenirovka /{ }");
            }
            else
                MessageBox.Show("Сначала выберете упражнение");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if(Trenya.SelectedItem != null)
            {
                if (Dostupnie_Treni[MyTrenigovki.SelectedIndex].ID_Sozdatel == Polzovatel.ID_User)
                {
                    Edit_Upraznenie a = new Edit_Upraznenie(Dostupnie_Uprazneniya[Trenya.SelectedIndex], Dostupnie_Treni[MyTrenigovki.SelectedIndex].Name, Polzovatel);
                    a.Top = this.Top;
                    a.Left = this.Left;
                    a.Show();
                    this.Hide();
                }
                else
                    MessageBox.Show("Вы не являетесь владельцем данной тренировки и не можете её изменить");
            }
            else
                MessageBox.Show("Сначала выберете упражнение");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (MyTrenigovki.SelectedItem != null)
            {
                client.Delete($"Users/{Polzovatel.ID_User}/Trenirovka/{Dostupnie_Treni[MyTrenigovki.SelectedIndex].Name}");
                MessageBox.Show("Данные удалены");
                Trenya.SelectedItem = null;
                FillListBox();
            }
            else
                MessageBox.Show("Сначала выберете, какую тренировку хотите удалить");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Adding_Trenirovka a = new Adding_Trenirovka(Polzovatel);
            a.Top = this.Top;
            a.Left = this.Left;
            a.Show();
            this.Hide();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (MyTrenigovki.SelectedItem != null)
            {
                if (Dostupnie_Treni[MyTrenigovki.SelectedIndex].Name != "Личная тренировка")
                {
                    if (Dostupnie_Treni[MyTrenigovki.SelectedIndex].ID_Sozdatel == Polzovatel.ID_User)
                    {
                        Export_Trenirovka a = new Export_Trenirovka(Polzovatel, Dostupnie_Treni[MyTrenigovki.SelectedIndex]);
                        a.Top = this.Top;
                        a.Left = this.Left;
                        a.Show();
                        this.Hide();
                    }
                    else
                        MessageBox.Show("Вы не создатель данной карты и не имеете права её экспортировать");
                }
                else
                    MessageBox.Show("Нельзя экспортировать личную тренировку, которая создаётся автоматически");
            }
            else
                MessageBox.Show("Сначала выберете, какую тренировку вы хотите экспортировать");
        }

        private void AllTrenirovki_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LichnieTrenirovki trenirovki = Vsse_Treni[AllTrenirovki.SelectedIndex];
                name_LT.Text = trenirovki.Name;
                opis_LT.Text = trenirovki.Opisanie;
                rayting_LT.Text = trenirovki.Rating.ToString();
            }
            catch
            {
                ClearTextBLockPart3();
            }
        }

        private void ClearTextBLockPart3()
        {
            name_LT.Text = "";
            opis_LT.Text = "";
            rayting_LT.Text = "";
            ochenka.Text = "";
        }

        private void opis_LT_Copy_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "012345".IndexOf(e.Text) < 0;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(ochenka.Text) >= 0)
            {
                if (Convert.ToInt32(ochenka.Text) <= 5)
                {
                    if (AllTrenirovki.SelectedItem != null)
                    {
                        LichnieTrenirovki trenirovki = Vsse_Treni[AllTrenirovki.SelectedIndex];
                        trenirovki.Rating = GetBall(trenirovki.CountOfVotes, Convert.ToInt32(ochenka.Text));
                        trenirovki.CountOfVotes.Add(Convert.ToInt32(ochenka.Text));
                        var setter = client.Set($"ExportedTrenirovku/{trenirovki.Name}", trenirovki);
                        MessageBox.Show("Вы оценили тренировку");
                        FillListBox();
                    }
                    else
                        MessageBox.Show("Сначала выберете тренировку, чтобы её оценить");
                }
                else
                    MessageBox.Show("Оценка не может быть больше 5");
            }
            else
                MessageBox.Show("Оценка не может быть меньше 0");
        }
        private double GetBall(List<int> ochenki, int NewOchenka)
        {
            int kolichestvo = 1;
            double obshiyBall = 0;
            foreach(int och in ochenki)
            {
                kolichestvo++;
                obshiyBall += och;
            }
            obshiyBall += NewOchenka;
            obshiyBall = obshiyBall / kolichestvo;
            return obshiyBall;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AllTrenirovki.SelectedItem != null)
                {
                    if (CheckInMyList() == true)
                    {
                        MessageBoxResult result = MessageBox.Show("В списке ваших тренировок уже есть тренировка с данным названием.\n" +
                            "В случае согласие на данной действие, данная выбранная тренировка будет замена на ту, что хранится в ваших тренировках.\nВы согласны на данное действие?", "Тренировка",
                         MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            LichnieTrenirovki trenirovkiA = Vsse_Treni[AllTrenirovki.SelectedIndex];
                            var setterA = client.Set($"Users/{Polzovatel.ID_User}/Trenirovka/{trenirovkiA.Name}", trenirovkiA);
                            MessageBox.Show("Данне добавлены!");
                            FillListBox();
                        }
                        else
                            MessageBox.Show("Отмена действия");
                    }

                    LichnieTrenirovki trenirovki = Vsse_Treni[AllTrenirovki.SelectedIndex];
                    var setter = client.Set($"Users/{Polzovatel.ID_User}/Trenirovka/{trenirovki.Name}", trenirovki);
                    MessageBox.Show("Данне добавлены!");
                    FillListBox();

                }
                else
                    MessageBox.Show("Сначала выберете тренировку, чтобы её добавить к себе");
            }
            catch
            {

            }
        }

        private bool CheckInMyList()
        {
            FirebaseResponse res = client.Get($"Users/{Polzovatel.ID_User}/Trenirovka/");
            Dictionary<string, LichnieTrenirovki> data = JsonConvert.DeserializeObject<Dictionary<string, LichnieTrenirovki>>(res.Body.ToString());
            if (data != null)
            {
                foreach (var a in data)
                {
                    if (a.Value.Name == Vsse_Treni[AllTrenirovki.SelectedIndex].Name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
    }
}
