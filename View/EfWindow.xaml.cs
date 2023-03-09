using ADO_202.EFCore;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X500;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace ADO_202.View
{
    /// <summary>
    /// Interaction logic for EfWindow.xaml
    /// </summary>
    public partial class EfWindow : Window
    {
        private EfContext efContext;
        private ICollectionView DepartmentsListView;   // інтерфейс для доступу до DepartmentsList з можливістю фільтрації
        private static readonly Random random = new();

        public EfWindow()
        {
            InitializeComponent();
            efContext = new();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // У EFCore - Lazy load - дані не завантажені до запитів
            efContext.Departments.Load();   // завантаження даних
            DepartmentsList.ItemsSource =
                efContext.Departments.Local.ToObservableCollection();

            // після зв'язування колекції та представлення, отримуємо інтерфейс ICollectionView
            DepartmentsListView = CollectionViewSource
                .GetDefaultView(DepartmentsList.ItemsSource);

            // задаємо фільтр через цей інтерфейс
            DepartmentsListView.Filter =   // Predicate<object>
                obj => (obj as Department)?.DeleteDt == null;  // TODO: replace with HideDeletedDepartmentsFilter

            UpdateMonitor();
        }
        private void UpdateMonitor()
        {
            MonitorBlock.Text = "Departments: " + efContext.Departments.Count();
            MonitorBlock.Text += "\nProducts: " + efContext.Products.Count();
            MonitorBlock.Text += "\nManagers: " + efContext.Managers.Count();
            MonitorBlock.Text += "\nSales: " + efContext.Sales.Count();
        }
        
        // Завдання: реалізувати обчислення денної статистики 
        private void UpdateDailyStatistics()
        {
            // Статистика продажів за сьогодні:
            // загалом продажів (чеків, записів у Sales) за сьогодні (усіх, у т.ч. видалених)
            SalesChecks.Content = "0";
            // загальна кількість проданих товарів (сума)
            SalesCnt.Content = "0";
            // фактичний час старту продажів сьогодні
            StartMoment.Content = "00:00:00";
            // час останнього продажу
            FinishMoment.Content = "00:00:00";
            // максимальна кількість товарів у одному чеку (за сьогодні)
            MaxCheckCnt.Content = "0";
            // "середній чек" за кількістю - середнє значення кількості 
            //  проданих товарів на один чек
            AvgCheckCnt.Content = "0.0";
            // Повернення - чеки, що є видаленими (кількість чеків за сьогодні)
            DeletedCheckCnt.Content = "0";
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
            if (dialog.ShowDialog() == true)
            {
                // dialog.EditedDepartment - Entity.Department
                efContext.Departments.Add(
                    new Department()   // EfCore.Department
                    {
                        Id = dialog.EditedDepartment.Id,
                        Name = dialog.EditedDepartment.Name
                    });

                // !! Зміна колекцій контексту не вносить зміни у БД (і у контекст) - 
                //    тільки планує ці зміни
                efContext.SaveChanges();   // фіксація усіх змін

                MonitorBlock.Text += "\nDepartments: " +
                    efContext.Departments.Count();
            }
        }
        private bool HideDeletedDepartmentsFilter(object item)
        {
            if(item is Department department)
            {
                return department.DeleteDt is null;
            }
            return false;
        }
        private void ShowDeletedDepartmentsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            DepartmentsListView.Filter = null;  // "скидаємо" фільтр - показує усе
            ((GridView)DepartmentsList.View)    // "відображаємо" колонку DeleteDt
                .Columns[2]                     // знаходимо її через View
                .Width = Double.NaN;            // та встановлюємо авто-ширину
        }
        private void ShowDeletedDepartmentsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            DepartmentsListView.Filter = HideDeletedDepartmentsFilter;
            ((GridView)DepartmentsList.View).Columns[2].Width = 0;
        }
        private void AddSalesButton_Click(object sender, RoutedEventArgs e)
        {
            // Завдання: створити 100 продажів з умовами:
            // продавець-менеджер вибирається випадково (з наявних у контексті),
            // товар - так само, час продажу - випадковий у межах двох днів (вчора-сьогодні)
            // кількість - видакова, але граничне значення: 1-2 для найдорожчих товарів,
            // 100 - для найдешевших (з плавним переходом за ціною)
            // DeleteDt: випадковий з ймовірністю 1/50, значення - не менше ніж час продажу

            int manCnt = efContext.Managers.Count();
            int proCnt = efContext.Products.Count();
            double maxPrice = efContext.Products.Max(p => p.Price);
                
            for (int i = 0; i < 100; i++)
            {
                // вправа - вивести випадкового співробітника (з наявних у контексті)
                int randIndex = random.Next(manCnt);
                Manager manager = efContext.Managers.Skip(randIndex).First();
                // MessageBox.Show(manager.Surname + " " + manager.Name);

                // випадковий товар
                randIndex = random.Next(proCnt);
                Product product = efContext.Products.Skip(randIndex).First();

                // час продажу - випадковий у межах двох днів
                DateTime moment =
                    DateTime                               // 
                        .Today                             // сьогодні 00:00:00
                        .AddHours(8)                       // початок продажів (08:00)
                        .AddSeconds(random.Next(43200))    // випадк. у інтервалі 12 годин
                        .AddDays(-random.Next(2));         // випадк. зміщення дня (на 0 / -1)

                // кількість - видакова, залежить від ціни товару
                // лінійна зміна від 102 при ціні 0 до 2 при макс. ціні
                int cntLimit = (int)(100 * (1 - product.Price / maxPrice) + 2);
                int cnt = random.Next(1, cntLimit);  // ліміт не вкл. - від 1 до (101..1)

                // DeleteDt: випадковий з ймовірністю 1/50, значення - не менше ніж час продажу
                DateTime? deleteDt = random.Next(50) == 0
                                        ? moment.AddHours(random.Next(1, 48))
                                        : null;

                efContext.Sales.Add(new()
                {
                    Id = Guid.NewGuid(),
                    ManagerId = manager.Id,
                    ProductId = product.Id,
                    Cnt = cnt,
                    SaleDt = moment,
                    DeleteDt = deleteDt
                });
            }
            efContext.SaveChanges();
            UpdateMonitor();            

            /* Загальне:
             * Робота з DbContext "під капотом" переводиться у SQL.
             * Конфлікт може виникати у тому, що DbSet, як колекції, приймають усі їх
             * методи-розширення, але не всі з них можуть транслюватись у SQL
             * Наприклад, немає доступу до колекції за індексом
             *   спроба використати колекційний метод .ElementAt(index)
             *   призведе до InvalidOperationException: 'The LINQ expression ... could not be translated
             *   це означає, що немає SQL аналогу для зазначеної інструкції
             *   
             * Обмеження, що має ця технологія, називають LINQ-to-Entity (раніше LINQ-to-SQL)
             * 
             * Спроби звести DbSet до колекцій [ .ToList() ] нівелюють одну з головних
             * переваг БД - робота з BigData. Вживання .ToList() не забороняється, але
             * обмежується тими випадками, коли вибірка гарантовано невелика, і не рекомендується
             * для самих таблиць [ Products.ToList() ]
             */
        }
    }
}
/* Реалізувати видалення департаменту через діалогове вікно (CRUD).
 * Пересвідчитись, що після видалення відділ зникає з переліку.
 */
/*
 * Т.З. Урахування видаленого контенту.
 * - При старті вікна не відображати ті департаменти, що є видаленими (встановлена
 *     дата видалення)
 * - Додати чекбокс "показувати видалені" відмітка якого
 *  = Додасть до переліку видалені відділи
 *  = Додасть колонку "Дата видалення"
 * - Зняття відмітки поверне вікно до початкового стану
 * 
 * Аналіз:
 * Видалені департаменти є частиною даних, відповідно їх необхідно фільтрувати
 * варіанти:
 * а)  ....ToObservableCollection().Where(d => d.DeleteDt is null);
 *     у такій реалізації .Where утворить нову колекцію, яка втратить зв'язок з
 *     початковими даними. Для оноволення вікна доведеться повторювати
 *     цю команду
 * б) застосувати фільтри на рівні View (при відображенні даних), а самі дані
 *     ніяк не фільтрувати - краще через економію на перетворенні даних
 *     Прямого доступу до фільтрів View немає, необхідно змінювати інтерфейс керування
 *     
 * Приховування/відображення колонки ListView
 * Спеціальних атрібутів для цього немає (IsVisible etc)
 * Задача вирішується встановленням нульової/ненульової ширини
 */