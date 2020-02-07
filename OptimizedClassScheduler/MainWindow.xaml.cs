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
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;

namespace OptimizedClassScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private class TeacherDataType
        {
            public string TeacherShortName { get; set; }
            public string TeacherName { get; set; }
            public string TeacherDept { get; set; }
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

        private class ShowCourseDataType
        {
            public string Code { get; set; }
            public string Title { get; set; }
            public string Year { get; set; }
            public string Semester { get; set; }
            public string Teacher1 { get; set; }
            public string Teacher2 { get; set; }
            public int Credit { get; set; }
        }

        private class ShowTeacherDataType
        {
            public string Department { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
        }

        private class YearBasedCourseDataType
        {
            public int CourseCredit { get; set; }
            public string CourseCode { get; set; }
            public Tuple<int, int> TeacherCompletedCredit { get; set; } 
        }

        private int totalCourses;
        readonly private int totalDay = 5;
        readonly private int totalPeriod = 8;
        readonly private string teachersJsonFilePath = "data/teachers.json";  //TODO: use correct json path
        readonly private string coursesJsonFilePath = "data/courses.json";
        readonly private string[] numberToString = new string[] { "Test", "First", "Second", "Third", "Fourth" };
        private List<ShowCourseDataType> showCoursesList = new List<ShowCourseDataType>();
        private List<YearBasedCourseDataType>[] yearBasedCourses = new List<YearBasedCourseDataType>[6] { new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>(), new List<YearBasedCourseDataType>() };
        private Dictionary<string, TeacherDataType> teachers = new Dictionary<string, TeacherDataType>();
        Dictionary<string, CourseDataType> courses = new Dictionary<string, CourseDataType>();

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DasboardButtonClick(object sender, RoutedEventArgs e)
        {
            editCoursesRectangle.Visibility = Visibility.Hidden;
            editTeachersRectangle.Visibility = Visibility.Hidden;
            dashboardRectangle.Visibility = Visibility.Visible;
            
            editTeachersGrid.Visibility = Visibility.Hidden;
            editCoursesGrid.Visibility = Visibility.Hidden;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private void EditTeachersButtonClick(object sender, RoutedEventArgs e)
        {
            editCoursesRectangle.Visibility = Visibility.Hidden;
            dashboardRectangle.Visibility = Visibility.Hidden;
            editTeachersRectangle.Visibility = Visibility.Visible;

            editCoursesGrid.Visibility = Visibility.Hidden;
            dashboardGrid.Visibility = Visibility.Hidden;
            editTeachersGrid.Visibility = Visibility.Visible;
        }

        private void EditCoursesButtonClick(object sender, RoutedEventArgs e)
        {
            editTeachersRectangle.Visibility = Visibility.Hidden;
            dashboardRectangle.Visibility = Visibility.Hidden;
            editCoursesRectangle.Visibility = Visibility.Visible;

            dashboardGrid.Visibility = Visibility.Hidden;
            editTeachersGrid.Visibility = Visibility.Hidden;
            editCoursesGrid.Visibility = Visibility.Visible;
        }

        private void TeacherSaveButtonClick(object sender, RoutedEventArgs e)
        {
            teachers[TeacherShortNameTextBox.Text] = new TeacherDataType
                                                    {
                                                        TeacherName = TeacherNameTextBox.Text,
                                                        TeacherDept = TeacherDeptTextBox.Text,
                                                        TeacherShortName = TeacherShortNameTextBox.Text
                                                    };
            string jsonData = JsonConvert.SerializeObject(teachers);

            //TODO: Set that jsonData into a file
        }

        private void CourseSaveButtonClick(object sender, RoutedEventArgs e)
        {
            string currentCourseCode = editCoursesCourseCodeTextTextBox.Text + editCoursesCourseCodeNumberTextBox.Text;
            courses[currentCourseCode] = new CourseDataType
            {
                CourseCode = currentCourseCode,
                CourseTitle = editCoursesCourseTitleTextBox.Text,
                CourseCredit = Int32.Parse(editCoursesCourseCreditTextBox.Text),
                CourseYear = editCoursesCourseYearComboBox.SelectedIndex,
                CourseSemester = editCoursesCourseSemesterComboBox.SelectedIndex,
                CourseTeacher1 = editCoursesCourseTeacher1ComboBox.SelectedValue,
                CourseTeacher2 = editCoursesCourseTeacher2ComboBox.SelectedValue
            };
            string jsonData = JsonConvert.SerializeObject(courses);

            showCoursesList.Add(new ShowCourseDataType
            {
                Code = courses[currentCourseCode].CourseCode,
                Title = courses[currentCourseCode].CourseTitle,
                Year = numberToString[courses[currentCourseCode].CourseYear],
                Semester = numberToString[courses[currentCourseCode].CourseSemester],
                Credit = courses[currentCourseCode].CourseCredit,
                Teacher1 = teachers[courses[currentCourseCode].CourseTeacherShortName.Item1].TeacherName,
                Teacher2 = teachers[courses[currentCourseCode].CourseTeacherShortName.Item2].TeacherName
            });
            courseDataGrid.ItemsSource = ;
            //TODO: Set that jsonData into a file
        }

        private void DifferntiateCourses(int currentSemester)
        {
            totalCourses = 0;
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
                    totalCourses++;
                }
            }
        }

        private void ShowRoutine(List<Tuple<string, int>>[,] routine)
        {
            routineGrid.Visibility = Visibility.Visible;
            routineDataGrid.ItemsSource = routine;
        }

        private void CreateRoutineButtonClick(object sender, RoutedEventArgs e)
        {
            int processedCourses = 0, totalCourses = 0,
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
                        }
                    }
                }
            }

            while (processedCourses < totalCourses)
            {
                for (int day = 0; day < totalDay; day++)
                {
                    int lastClassYear = 1;
                    for (int period = 0; period < totalPeriod; period++)
                    {
                        int addedCourseIndex = new int();
                        bool foundClass = false;

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
                            processedCourses++;

                            yearBasedCourses[lastClassYear][addedCourseIndex] = currentYearBasedCourse;

                            if (currentYearBasedCourse.TeacherCompletedCredit.Item1 + currentYearBasedCourse.TeacherCompletedCredit.Item2 >= courses[currentYearBasedCourse.CourseCode].CourseCredit)
                            {
                                yearBasedCourses[lastClassYear].RemoveAt(addedCourseIndex);
                            }
                        }
                    }

                }
            }

            //TODO: Create GRAPHICS
            ShowRoutine(routine);
        }

        private void DashboardSemesterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dashboardSemesterComboBox.SelectedIndex != 0)
            {
                DifferntiateCourses(dashboardSemesterComboBox.SelectedIndex);

                dashboardDataGrid.ItemsSource = yearBasedCourses;
            }
            else
            {
                //dashboardTreeView.DataContext = null;
                //TODO: Clear treeview
            }
        }

        private void RoutineRectangleMouseClick(object sender, MouseButtonEventArgs e)
        {
            routineGrid.Visibility = Visibility.Hidden;
        }
    }
}