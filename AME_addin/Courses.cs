using AME_addin.Properties;
using EF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows;

namespace AME_addin
{
    public class Courses
    {
        private Collection<CourseWrapper> _courses;
        public Courses()
        { 
            string[] str = new string[Settings.Default.courseNames.Count];
            Settings.Default.courseNames.CopyTo(str, 0);
            _courses = new Collection<CourseWrapper>(str.Select(cn => new CourseWrapper(cn)).ToList());
        }

        public CourseContext GetCourse(string courseName, EventHandler onFinishedEventHandler, bool createIfNotFound)
        {
            CourseWrapper foundCourse;
            foundCourse = _courses.FirstOrDefault(c => c.name == courseName);
            if (foundCourse == null)
            {
                foundCourse = new CourseWrapper(courseName);
                _courses.Add(foundCourse);
            }
            if (foundCourse == null)
                throw new ArgumentNullException();
            onFinishedEventHandler += foundCourse.releaseHandler;
            return createIfNotFound ? foundCourse.createContext() : foundCourse.context;
        }

        //contains dbcontext and other stuff
        private class CourseWrapper : IDisposable
        {
            public string name;
            public CourseWrapper(string paramName)
            {
                name = paramName;
            }

            public CourseContext createContext()
            {
                _references = 1;
                _context = new CourseContext(sqlconnection);
                return _context;
            }

            private CourseContext _context;
            public CourseContext context
            {
                get
                {
                    if (_context == null)
                    {
                        if (!System.IO.File.Exists(SQLCompactDBPath))
                            System.Windows.MessageBox.Show(SQLCompactDBPath + " not found. Creating new class database...", "?", MessageBoxButton.OK);
                        _context = new CourseContext(sqlconnection);
                        _references = 0;
                    }
                    _references++;
                    return _context;
                }
            }

            private int _references = 0;
            public void releaseHandler(object sender, System.EventArgs e)
            {
                _references--;
                if (_references < 1)
                    _context = null;
            }

            private SqlCeConnection _sqlconnection;
            public SqlCeConnection sqlconnection
            {
                get
                {
                    if (_sqlconnection == null)
                        _sqlconnection = new SqlCeConnection(connectionString);
                    return _sqlconnection;
                }
            }


            public void Dispose()
            {
                if (_context != null)
                    sqlconnection.Close();
                _context.Dispose();
            }

            public string SQLCompactDBPath 
            { 
                get 
                { 
                    return  Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + name + ".sdf"; 
                } 
            }
            public string connectionString { get { return "Data Source=" + SQLCompactDBPath + ";Persist Security Info=False;"; } }

            public void compact()
            {
                new SqlCeEngine().Compact(connectionString);
            }

            public void purge()
            {
                if (System.Windows.MessageBox.Show("are you sure you want to delete ALL MARKBOOK DATA?", "?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    System.IO.File.Delete(SQLCompactDBPath);
                    System.Windows.MessageBox.Show(System.IO.File.Exists(SQLCompactDBPath) ? "delete failed" : "delete succeeded");
                    //using (Course db = new Course(true)) 
                    //{
                    //    db.Database.Initialize(true); //IMPORTANT! 
                    //}
                    //System.Windows.MessageBox.Show(System.IO.File.Exists(Course.DataBasePathFromName) ? "database creation succeeded" : "database creation failed");
                }
            }
        }
    }

}
