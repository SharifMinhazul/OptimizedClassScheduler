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
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.ObjectModel;


namespace OptimizedClassScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow
    {
        private class TeacherDataType
        {
            public string TeacherName { get; set; }
            public string TeacherDept { get; set; }
            public string TeacherShortName { get; set; }
        }

        private class CourseDataType
        {
            public int CourseYear { get; set; }
            public int CourseCredit { get; set; }
            public int CourseSemester { get; set; }
            public string CourseTitle { get; set; }
            public string CourseCode { get; set; }
            public Tuple<string, string> CourseTeacherShortName { get; set; }
        }

        private class YearBasedCourseDataType
        {
            public int CourseCredit { get; set; }
            public string CourseCode { get; set; }
            public Tuple<int, int> TeacherCompletedCredit { get; set; }
        }

        readonly private int totalDay = 5;
        readonly private int totalPeriod = 8;
        readonly private string teachersJsonFilePath = "teachers.json";  //TODO: use correct json path
        readonly private string coursesJsonFilePath = "courses.json";
        private List<YearBasedCourseDataType>[] yearBasedCourses = new List<YearBasedCourseDataType>[6] { new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>() };
        private Dictionary<string, TeacherDataType> teachers = new Dictionary<string, TeacherDataType>();
        Dictionary<string, CourseDataType> courses = new Dictionary<string, CourseDataType>();

        private ObservableCollection<TeacherDataType> teacherList = new ObservableCollection<TeacherDataType>();
        private ObservableCollection<CourseDataType> courseList = new ObservableCollection<CourseDataType>();
        private ObservableCollection<CourseDataType> dashboardCourseList = new ObservableCollection<CourseDataType>();


        public MainWindow()
        {
            InitializeComponent();

            using (StreamWriter w = File.AppendText(coursesJsonFilePath)) {}
            using (StreamWriter v = File.AppendText(teachersJsonFilePath)) {}

            Dictionary<string, CourseDataType> jsonCourses = JsonConvert.DeserializeObject<Dictionary<string, CourseDataType>>(File.ReadAllText(coursesJsonFilePath));
            Dictionary<string, TeacherDataType> jsonTeachers = JsonConvert.DeserializeObject<Dictionary<string, TeacherDataType>>(File.ReadAllText(teachersJsonFilePath));

            if (jsonCourses != null)
            {
                courses = jsonCourses;
                courseList = new ObservableCollection<CourseDataType>(courses.Values);
            }
            if (jsonTeachers != null)
            {
                teachers = jsonTeachers;
                teacherList = new ObservableCollection<TeacherDataType>(teachers.Values);
            }

            this.DataContext = this;
            courseDataGrid.ItemsSource = courseList;
            teacherDataGrid.ItemsSource = teacherList;
            dashboardDataGrid.ItemsSource = dashboardCourseList;
            editCoursesCourseTeacher1ComboBox.ItemsSource = teacherList;
            editCoursesCourseTeacher2ComboBox.ItemsSource = teacherList;
        }
        
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DasboardButtonClick(object sender, RoutedEventArgs e)
        {
            dashboardRectangle.Visibility = Visibility.Visible;
            editTeachersRectangle.Visibility = Visibility.Hidden;
            editCoursesRectangle.Visibility = Visibility.Hidden;

            dashboardButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB9004C"));
            dashboardButton.Foreground = Brushes.White;
            editCoursesButton.Background = Brushes.Transparent;
            editCoursesButton.Foreground = Brushes.Black;
            editTeachersButton.Background = Brushes.Transparent;
            editTeachersButton.Foreground = Brushes.Black;

            editTeachersGrid.Visibility = Visibility.Hidden;
            editCoursesGrid.Visibility = Visibility.Hidden;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private void EditTeachersButtonClick(object sender, RoutedEventArgs e)
        {
            dashboardRectangle.Visibility = Visibility.Hidden;
            editTeachersRectangle.Visibility = Visibility.Visible;
            editCoursesRectangle.Visibility = Visibility.Hidden;

            dashboardButton.Background = Brushes.Transparent;
            dashboardButton.Foreground = Brushes.Black;
            editCoursesButton.Background = Brushes.Transparent;
            editCoursesButton.Foreground = Brushes.Black;
            editTeachersButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB9004C"));
            editTeachersButton.Foreground = Brushes.White;

            editCoursesGrid.Visibility = Visibility.Hidden;
            dashboardGrid.Visibility = Visibility.Hidden;
            editTeachersGrid.Visibility = Visibility.Visible;
        }

        private void EditCoursesButtonClick(object sender, RoutedEventArgs e)
        {
            dashboardRectangle.Visibility = Visibility.Hidden;
            editTeachersRectangle.Visibility = Visibility.Hidden;
            editCoursesRectangle.Visibility = Visibility.Visible;

            dashboardButton.Background = Brushes.Transparent;
            dashboardButton.Foreground = Brushes.Black;
            editCoursesButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB9004C"));
            editCoursesButton.Foreground = Brushes.White;
            editTeachersButton.Background = Brushes.Transparent;
            editTeachersButton.Foreground = Brushes.Black;

            dashboardGrid.Visibility = Visibility.Hidden;
            editTeachersGrid.Visibility = Visibility.Hidden;
            editCoursesGrid.Visibility = Visibility.Visible;
        }

        private void TeacherSaveButtonClick(object sender, RoutedEventArgs e)
        {
            TeacherDataType currentTeacher = new TeacherDataType
            {
                TeacherName = TeacherNameTextBox.Text,
                TeacherDept = TeacherDeptTextBox.Text,
                TeacherShortName = TeacherShortNameTextBox.Text
            };

            teachers[TeacherShortNameTextBox.Text] = currentTeacher;
            teacherList.Add(currentTeacher);

            TeacherNameTextBox.Text = string.Empty;
            TeacherDeptTextBox.Text = string.Empty;
            TeacherShortNameTextBox.Text = string.Empty;

            string jsonData = JsonConvert.SerializeObject(teachers);
            File.WriteAllText(teachersJsonFilePath, jsonData);
        }

        private void CourseSaveButtonClick(object sender, RoutedEventArgs e)
        {
            string currentCourseCode = editCoursesCourseCodeTextTextBox.Text + editCoursesCourseCodeNumberTextBox.Text;
            CourseDataType currentCourse = new CourseDataType
            {
                CourseCode = currentCourseCode,
                CourseTitle = editCoursesCourseTitleTextBox.Text,
                CourseCredit = Int32.Parse(editCoursesCourseCreditTextBox.Text),
                CourseYear = editCoursesCourseYearComboBox.SelectedIndex,
                CourseSemester = editCoursesCourseSemesterComboBox.SelectedIndex,
                //CourseTeacherShortName = Tuple.Create("No Name", "No Name")
                CourseTeacherShortName = new Tuple<string, string>(
                    editCoursesCourseTeacher1ComboBox.Text,
                    editCoursesCourseTeacher2ComboBox.Text
                )
            };

            courses[currentCourseCode] = currentCourse;
            courseList.Add(currentCourse);

            editCoursesCourseTitleTextBox.Text = string.Empty;
            editCoursesCourseCodeTextTextBox.Text = string.Empty;
            editCoursesCourseCodeNumberTextBox.Text = string.Empty;
            editCoursesCourseCreditTextBox.Text = string.Empty;
            editCoursesCourseYearComboBox.SelectedIndex = 0;
            editCoursesCourseSemesterComboBox.SelectedIndex = 0;
            editCoursesCourseTeacher1ComboBox.SelectedIndex = 0;
            editCoursesCourseTeacher2ComboBox.SelectedIndex = 0;

            string jsonData = JsonConvert.SerializeObject(courses);
            File.WriteAllText(coursesJsonFilePath, jsonData);
        }

        private void DifferntiateCourses(int currentSemester)
        {
            dashboardCourseList.Clear();
            for (int year = 1; year <= 4; year++)
            {
                yearBasedCourses[year].Clear();
            }

            foreach (KeyValuePair<string, CourseDataType> currentCourse in courses)
            {
                if (currentCourse.Value.CourseSemester == currentSemester)
                {
                    yearBasedCourses[currentCourse.Value.CourseYear].Add(new YearBasedCourseDataType
                    {
                        CourseCode = currentCourse.Value.CourseCode,
                        CourseCredit = currentCourse.Value.CourseCredit,
                        TeacherCompletedCredit = Tuple.Create(0, 0)
                    });
                    dashboardCourseList.Add(currentCourse.Value);
                }
            }
        }
       
        private void ShowRoutine(List<Tuple<string, int>>[,] routine)
        {
            routineSemesterText.Text = dashboardSemesterComboBox.Text +  " Routine";

            routineGrid.Visibility = Visibility.Visible;
            mainGrid.Effect = new BlurEffect() { Radius = 15 };


            List<List<string>> strRoutine = new List<List<string>>();

            for (int day = 0, idx = 0; day < totalDay; day++)
            {
                for (int classroom = 0; classroom < 5; classroom++)
                {
                    bool needNewClassroom = false;
                    for (int period = 0; period < totalPeriod; period++)
                    {
                        if (routine[day, period].Count() > classroom)
                        {
                            needNewClassroom = true;
                            break;
                        }
                    }

                    if (needNewClassroom) {
                        strRoutine.Add(new List<string>());
                        strRoutine[idx].Add("Classroom " + (classroom + 1).ToString());
                        for (int period = 0; period < totalPeriod; period++)
                        {
                            //strRoutine[i].Add("Working");
                            if (routine[day, period].Count() > classroom)
                            {
                                int op = routine[day, period][classroom].Item2;
                                string courseCode = routine[day, period][classroom].Item1, teacherName;

                                if (op == 0) {
                                        teacherName = courses[courseCode].CourseTeacherShortName.Item1;
                                } 
                                else if (op == 1)
                                {
                                    teacherName = courses[courseCode].CourseTeacherShortName.Item2;
                                }
                                else
                                {
                                    teacherName = courses[courseCode].CourseTeacherShortName.Item1 + " \\ "
                                         + courses[courseCode].CourseTeacherShortName.Item2;
                                }

                                strRoutine[idx].Add(courseCode + "\n" + teacherName);
                            }
                            else
                            {
                                strRoutine[idx].Add(" ");
                            }
                        }
                        idx++;
                    }
                }
            }

            routineDataGrid.ItemsSource = strRoutine;
        }

        private void CreateRoutineButtonClick(object sender, RoutedEventArgs e)
        {
            if (dashboardSemesterComboBox.SelectedIndex != 0)
            {
                DifferntiateCourses(dashboardSemesterComboBox.SelectedIndex);
            }

            int processedCourses = 0, totalCourses = yearBasedCourses[1].Count() + yearBasedCourses[2].Count() + yearBasedCourses[3].Count() + yearBasedCourses[4].Count(),
                maxClassYear = Int32.Parse(maxClassYearTextBox.Text),
                maxClassTeacher = Int32.Parse(maxClassTeacherTextBox.Text);
            int[,] classCountYear = new int[4 + 2, totalDay + 2];
            bool[,,] hasClassYear = new bool[4 + 2, totalDay + 2, totalPeriod + 2];
            List<Tuple<string, int>>[,] routine = new List<Tuple<string, int>>[totalDay + 2, totalPeriod + 2];
            Dictionary<string, bool>[] hasClassCourse = new Dictionary<string, bool>[totalDay + 2];
            Dictionary<string, bool>[,] hasClassTeacher = new Dictionary<string, bool>[totalDay + 2, totalPeriod + 2];
            Dictionary<string, int>[] classCountTeacher = new Dictionary<string, int>[totalDay + 2];

            //Initialize data structures and reset value;
            for (int day = 0; day < totalDay; day++)
            {
                hasClassCourse[day] = new Dictionary<string, bool>();
                classCountTeacher[day] = new Dictionary<string, int>();
                for (int period = 0; period < totalPeriod; period++)
                {
                    hasClassTeacher[day, period] = new Dictionary<string, bool>();
                }

                for (int currentYear = 1; currentYear <= 4; currentYear++)
                {
                    foreach (YearBasedCourseDataType currentYearBasedCourse in yearBasedCourses[currentYear])
                    {
                        hasClassCourse[day][currentYearBasedCourse.CourseCode] = false;

                        for (int period = 0; period < totalPeriod; period++)
                        {
                            Tuple<string, string> currentTeacherShortName = courses[currentYearBasedCourse.CourseCode].CourseTeacherShortName;

                            hasClassYear[currentYear, day, period] = false;
                            hasClassTeacher[day, period][currentTeacherShortName.Item1] = false;
                            hasClassTeacher[day, period][currentTeacherShortName.Item2] = false;
                            classCountTeacher[day][currentTeacherShortName.Item1] = 0;
                            classCountTeacher[day][currentTeacherShortName.Item2] = 0;
                        }
                    }
                }
            }

            int countLoop = 0;
            MessageBox.Show("Total Courses -> " + totalCourses.ToString());
            while (processedCourses < totalCourses)
            {
                countLoop++;
                if (countLoop > 300)
                {
                    MessageBox.Show("Can't add some class any where :(");
                }
                MessageBox.Show(processedCourses.ToString() + " -> " + totalCourses.ToString());

                for (int day = 0; day < totalDay; day++)
                {
                    int lastClassYear = 1;
                    for (int period = 0; period < totalPeriod; period++)
                    {
                        int addedCourseIndex = new int();
                        bool foundClass = false;

                        if (routine[day, period] == null)
                        {
                            routine[day, period] = new List<Tuple<string, int>>();
                        }

                        if (classCountYear[lastClassYear, day] < maxClassYear && hasClassYear[lastClassYear, day, period] == false)
                        {
                            for (int currentIndex = 0; currentIndex < yearBasedCourses[lastClassYear].Count; currentIndex++)
                            {
                                ///<summary>
                                /// DONE: Check if course teachers don't have class this day and this period;
                                /// DONE: Check if course teachers don't have more class than max class;
                                /// DONE: Check if year doesn't have class this day and this period;
                                /// DONE: Check if year doesn't have more class than max class;
                                /// DONE: Check if this course doesn't have class this day;
                                /// DONE: Check teachers remaining credit for class;
                                ///</summary>
                                YearBasedCourseDataType currentYearBasedCourse = yearBasedCourses[lastClassYear][currentIndex];

                                if (hasClassCourse[day][currentYearBasedCourse.CourseCode] == false)
                                {
                                    Tuple<string, string> currentTeacher = courses[currentYearBasedCourse.CourseCode].CourseTeacherShortName;
                                    bool hasNoClassTeacher1 = hasClassTeacher[day, period][currentTeacher.Item1] == false,
                                         classCountOkTeacher1 = classCountTeacher[day][currentTeacher.Item1] < maxClassTeacher,
                                         creditOkTeacher1 = currentYearBasedCourse.TeacherCompletedCredit.Item1 < currentYearBasedCourse.CourseCredit / 2,

                                         hasNoClassTeacher2 = hasClassTeacher[day, period][currentTeacher.Item2] == false,
                                         classCountOkTeacher2 = classCountTeacher[day][currentTeacher.Item2] < maxClassTeacher,
                                         creditOkTeacher2 = currentYearBasedCourse.TeacherCompletedCredit.Item2 < currentYearBasedCourse.CourseCredit / 2;

                                    addedCourseIndex = currentIndex;
                                    if (hasNoClassTeacher1 && classCountOkTeacher1 && creditOkTeacher1)
                                    {
                                        routine[day, period].Add(Tuple.Create(currentYearBasedCourse.CourseCode, 0));
                                        hasClassTeacher[day, period][currentTeacher.Item1] = true;
                                        classCountTeacher[day][currentTeacher.Item1]++;
                                        currentYearBasedCourse.TeacherCompletedCredit = Tuple.Create(
                                            currentYearBasedCourse.TeacherCompletedCredit.Item1 + 1,
                                            currentYearBasedCourse.TeacherCompletedCredit.Item2
                                        );
                                        foundClass = true;
                                        break;
                                    } 
                                    else if (hasNoClassTeacher2 && classCountOkTeacher2 && creditOkTeacher2)
                                    {
                                        routine[day, period].Add(Tuple.Create(currentYearBasedCourse.CourseCode, 1));
                                        hasClassTeacher[day, period][currentTeacher.Item2] = true;
                                        classCountTeacher[day][currentTeacher.Item2]++;
                                        currentYearBasedCourse.TeacherCompletedCredit = Tuple.Create(
                                            currentYearBasedCourse.TeacherCompletedCredit.Item1,
                                            currentYearBasedCourse.TeacherCompletedCredit.Item2 + 1
                                        );
                                        foundClass = true;
                                        break;
                                    }
                                    else if (currentYearBasedCourse.CourseCredit == 3 && currentYearBasedCourse.TeacherCompletedCredit.Item1 + currentYearBasedCourse.TeacherCompletedCredit.Item2 == 2)
                                    {
                                        routine[day, period].Add(Tuple.Create(currentYearBasedCourse.CourseCode, 3));
                                        hasClassTeacher[day, period][currentTeacher.Item1] = true;
                                        hasClassTeacher[day, period][currentTeacher.Item2] = true;
                                        classCountTeacher[day][currentTeacher.Item1]++;
                                        classCountTeacher[day][currentTeacher.Item2]++;
                                        currentYearBasedCourse.TeacherCompletedCredit = Tuple.Create(
                                            currentYearBasedCourse.TeacherCompletedCredit.Item1 + 1,
                                            currentYearBasedCourse.TeacherCompletedCredit.Item2 + 1
                                        );
                                        foundClass = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (foundClass == false)
                        {
                            for (int currentYear = 1; currentYear <= 4 && foundClass == false; currentYear++)
                            {
                                lastClassYear = currentYear;
                                if (classCountYear[currentYear, day] < maxClassYear && hasClassYear[currentYear, day, period] == false)
                                {
                                    for (int currentIndex = 0; currentIndex < yearBasedCourses[currentYear].Count; currentIndex++)
                                    {
                                        ///<summary>
                                        /// DONE: Check if course teachers don't have class this day and this period;
                                        /// DONE: Check if course teachers don't have more class than max class;
                                        /// DONE: Check if year doesn't have class this day and this period;
                                        /// DONE: Check if year doesn't have more class than max class;
                                        /// DONE: Check if this course doesn't have class this day;
                                        /// DONE: Check teachers remaining credit for class;
                                        ///</summary>
                                        YearBasedCourseDataType currentYearBasedCourse = yearBasedCourses[currentYear][currentIndex];

                                        if (hasClassCourse[day][currentYearBasedCourse.CourseCode] == false)
                                        {
                                            Tuple<string, string> currentTeacher = courses[currentYearBasedCourse.CourseCode].CourseTeacherShortName;
                                            bool hasNoClassTeacher1 = hasClassTeacher[day, period][currentTeacher.Item1] == false,
                                                 classCountOkTeacher1 = classCountTeacher[day][currentTeacher.Item1] < maxClassTeacher,
                                                 creditOkTeacher1 = currentYearBasedCourse.TeacherCompletedCredit.Item1 < currentYearBasedCourse.CourseCredit / 2,

                                                 hasNoClassTeacher2 = hasClassTeacher[day, period][currentTeacher.Item2] == false,
                                                 classCountOkTeacher2 = classCountTeacher[day][currentTeacher.Item2] < maxClassTeacher,
                                                 creditOkTeacher2 = currentYearBasedCourse.TeacherCompletedCredit.Item2 < currentYearBasedCourse.CourseCredit / 2;
                                             
                                            addedCourseIndex = currentIndex;
                                            if (hasNoClassTeacher1 && classCountOkTeacher1 && creditOkTeacher1)
                                            {
                                                routine[day, period].Add(Tuple.Create(currentYearBasedCourse.CourseCode, 0));
                                                hasClassTeacher[day, period][currentTeacher.Item1] = true;
                                                classCountTeacher[day][currentTeacher.Item1]++;
                                                currentYearBasedCourse.TeacherCompletedCredit = Tuple.Create(
                                                    currentYearBasedCourse.TeacherCompletedCredit.Item1 + 1,
                                                    currentYearBasedCourse.TeacherCompletedCredit.Item2
                                                );
                                                foundClass = true;
                                                break;
                                            }
                                            else if (hasNoClassTeacher2 && classCountOkTeacher2 && creditOkTeacher2)
                                            {
                                                routine[day, period].Add(Tuple.Create(currentYearBasedCourse.CourseCode, 1));
                                                hasClassTeacher[day, period][currentTeacher.Item2] = true;
                                                classCountTeacher[day][currentTeacher.Item2]++;
                                                currentYearBasedCourse.TeacherCompletedCredit = Tuple.Create(
                                                    currentYearBasedCourse.TeacherCompletedCredit.Item1,
                                                    currentYearBasedCourse.TeacherCompletedCredit.Item2 + 1
                                                );
                                                foundClass = true;
                                                break;
                                            }
                                            else if (currentYearBasedCourse.CourseCredit == 3 && currentYearBasedCourse.TeacherCompletedCredit.Item1 + currentYearBasedCourse.TeacherCompletedCredit.Item2 == 2)
                                            {
                                                routine[day, period].Add(Tuple.Create(currentYearBasedCourse.CourseCode, 3));
                                                hasClassTeacher[day, period][currentTeacher.Item1] = true;
                                                hasClassTeacher[day, period][currentTeacher.Item2] = true;
                                                classCountTeacher[day][currentTeacher.Item1]++;
                                                classCountTeacher[day][currentTeacher.Item2]++;
                                                currentYearBasedCourse.TeacherCompletedCredit = Tuple.Create(
                                                    currentYearBasedCourse.TeacherCompletedCredit.Item1 + 1,
                                                    currentYearBasedCourse.TeacherCompletedCredit.Item2 + 1
                                                );
                                                foundClass = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (foundClass == true)
                        {
                            ///<summary>
                            /// DONE: Check if course teachers don't have class this day and this period;
                            /// DONE: Check if course teachers don't have more class than max class;
                            /// DONE: Check if year doesn't have class this day and this period;
                            /// TODO: Check if year doesn't have more class than max class;
                            /// DONE: Check if this course doesn't have class this day;
                            /// DONE: Check teachers remaining credit for class;
                            ///</summary>
                            YearBasedCourseDataType currentYearBasedCourse = yearBasedCourses[lastClassYear][addedCourseIndex];

                            classCountYear[lastClassYear, day]++;
                            hasClassYear[lastClassYear, day, period] = true;
                            hasClassCourse[day][currentYearBasedCourse.CourseCode] = true;

                            yearBasedCourses[lastClassYear][addedCourseIndex] = currentYearBasedCourse;

                            if (currentYearBasedCourse.TeacherCompletedCredit.Item1 + currentYearBasedCourse.TeacherCompletedCredit.Item2 >= courses[currentYearBasedCourse.CourseCode].CourseCredit)
                            {
                                yearBasedCourses[lastClassYear].RemoveAt(addedCourseIndex);
                                processedCourses++;
                            }
                        }
                        else
                        {
                            lastClassYear = 1;
                        }
                    }

                }
            }

            ShowRoutine(routine);
        }

        private void DashboardSemesterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dashboardSemesterComboBox.SelectedIndex != 0)
            {
                DifferntiateCourses(dashboardSemesterComboBox.SelectedIndex);
            }
            else
            {
                dashboardCourseList.Clear();
            }
        }

        private void RoutineRectangleMouseClick(object sender, MouseButtonEventArgs e)
        {
            routineGrid.Visibility = Visibility.Hidden;
            mainGrid.Effect = new BlurEffect() { Radius = 0 };
        }
    }

    public class CardinalToOrdinalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object Parameter, System.Globalization.CultureInfo culture)
        {
            return new string[] { "Test", "First", "Second", "Third", "Fourth" }[Int32.Parse(value.ToString())];
        }

        public object ConvertBack(object value, Type targetType, object Parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "Test":
                    return 0;
                case "First":
                    return 1;
                case "Second":
                    return 2;
                case "Third":
                    return 3;
                case "Fourth":
                    return 4;
                default:
                    return -1;
            }
        }
    }
}