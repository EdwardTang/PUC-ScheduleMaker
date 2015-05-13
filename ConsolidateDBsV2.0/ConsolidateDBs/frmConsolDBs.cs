using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Data.SQLite;
using HtmlAgilityPack;

namespace ConsolidateDBs
{
   
    public partial class frmConsolDBs : Form
    {
        private string sSelectedFolder;
        private string sSelectedDbName;
        private int checkPointNum;
        private string sSelectedSqlFolder;
        private string sSelectedSqlName;
        private string quarter;
        private int year;
        //string connString = "Data Source=..\\..\\..\\..\\..\\Database\\PUC_course_schedule.sqlite;Version=3;";
        string connString = "Data source=" + Application.StartupPath + @"\\PUC_course_schedule.sqlite";
        string roomNumber, subject, instructor, firstName, lastName, schedule, building, CRN, CRNLink, title, startDate, endDate, number, isTransfer, isEXL, isSIA, isETIE, section, crHrs, enrTaken, enrAvail, startTime, endTime, isOnline, isCancelled, waitTaken, waitAvail, textbookLink, notes, days;
        string building_list = "";
        string subject_list = "";
        string instructor_list = "";
        string schedule_list = "";
        string course_list = "";
        Int64 courseID;
        Int64 sectionID;
        List<string> PreReqList = new List<string>();
        List<string> CoReqList = new List<string>();

        
        //private string connStr;


        public frmConsolDBs()
        {
            InitializeComponent();
            
            //listBoxDBsMainTab1.Enabled = false;
            //tabControlTableCheckpoints.Enabled = false;
            dataGridView1.Enabled = false;
            tb_output.AppendText(DateTime.Now.ToShortTimeString() + System.Environment.NewLine);
            //checkedListBoxCourses.SelectionMode = System.Windows.Forms.SelectionMode.One;
        }

        // Process all files in the directory passed in, recurse on any directories  
        // that are found, and process the files they contain. 
        public string[]  ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory. 
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            return fileEntries;
        }

        private string GetConnectionStr()
        {
            return (@"Data Source=" + sSelectedFolder + Path.DirectorySeparatorChar + sSelectedDbName + ".sqlite;Version=3;");
        }

        private void UpdateDataGrid(string con, string sql)
        {
            SQLiteConnection dbConnection = null;

            SQLiteDataAdapter dataAdapter = null;
            //Display the SQL it run in rich text box
            rtxtBoxSQL.Text = sql;
            try
            {


                // DB code
                dbConnection = new SQLiteConnection(con);
                dbConnection.Open();
                dataAdapter = new SQLiteDataAdapter(sql, con);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                // non-DB code
                dataGridView1.DataSource = dataSet.Tables[0].DefaultView;

            }
            catch (SQLiteException sqle)
            {
                // Handle DB exception

                MessageBox.Show("Message: " + sqle.Message + "\n");
            }
            catch (IndexOutOfRangeException ie)
            {
                // If you think there might be a problem with index range in the loop, for example
            }
            catch (Exception ex)
            {
                // If you want to catch any exception that the previous catches don't catch (that is, if you want to handle other exceptions, rather than let them bubble up to the method caller)
                MessageBox.Show("Message: " + ex.Message + "\n");
            }
            finally
            {
                // I recommend doing some null-checking here, otherwise you risk a NullReferenceException.  There's nothing quite like throwing an exception from within a finally block for fun debugging.
                if (dbConnection != null)
                    dbConnection.Dispose();
                if (dataAdapter != null)
                    dataAdapter.Dispose();
            }



        }

  


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "Custom Description"; //not mandatory

            if (fbd.ShowDialog() == DialogResult.OK)
                sSelectedFolder = fbd.SelectedPath;
            else
                sSelectedFolder = string.Empty;

            txtDirectory1.Text = sSelectedFolder;

            listBoxDBsMainTab1.Enabled = true;
            listBoxDBsMainTab1.Items.Clear();

            if (Directory.Exists(txtDirectory1.Text))
            {

                string[] fileEntries = ProcessDirectory(txtDirectory1.Text);

                //listBoxDBsTab1.BeginUpdate();
                // Loop through and add 50 items to the ListBox. 
                foreach (string fileName in fileEntries)
                {
                    string extension;

                    extension = Path.GetExtension(fileName);
                    if (extension == ".sqlite")
                    {
                        string dbName;
                        dbName = Path.GetFileNameWithoutExtension(fileName);
                        listBoxDBsMainTab1.Items.Add(dbName);
                    }

                }

                // Allow the ListBox to repaint and display the new items.
                //listBoxDBsTab1.EndUpdate();
            }
            else
            {
                MessageBox.Show(txtDirectory1.Text + " is not a valid directory.");
            }
        }

        private void listBoxDBsMainTab1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControlTableCheckpoints.Enabled = true;
            sSelectedDbName = listBoxDBsMainTab1.Items[listBoxDBsMainTab1.SelectedIndex].ToString();
            
        }


        /// <summary>
        ///  Check if there is duplicates in Courses Table 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckDuplicates_Click(object sender, EventArgs e)
        {
            
            
            string connStr = GetConnectionStr();
            string termPostfix = "_"+sSelectedDbName;

            string courses = " Courses" + termPostfix;
            string subjects = " Subjects" + termPostfix;
            string cmdStr = "SELECT   S.subject, C.courseNum,C.credits, COUNT(*) AS duplicates"
                            + " FROM " + courses + " AS C"
                            + " JOIN " + subjects + " AS S ON S.subjectId = C.subjectId"
                            + " GROUP BY S.subject, C.courseNum, C.credits"
                            + " HAVING duplicates > 1"
                            + ";";

            dataGridView1.Enabled = true;
            lbDbName.Text = sSelectedDbName;
            UpdateDataGrid(connStr, cmdStr);

        }

        /// <summary>
        /// Check if there is empty subjectId in Courses table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckSubjectId_Click(object sender, EventArgs e)
        {
            string connStr = GetConnectionStr();
            string termPostfix = "_" + sSelectedDbName;
            string courses = " Courses" + termPostfix;
            string cmdStr = "SELECT COUNT(*) AS EmptySubjectId"
                           + " FROM " + courses + " AS C"
                           + " WHERE C.subjectId is null"
                           + ";";
            dataGridView1.Enabled = true;
            lbDbName.Text = sSelectedDbName;
            UpdateDataGrid(connStr, cmdStr);
        }

        private void btnCheckEmptySectionId1_Click(object sender, EventArgs e)
        {
            string connStr = GetConnectionStr();
            string termPostfix = "_" + sSelectedDbName;
            string sectionTimes = " SectionTimes" + termPostfix;
            string cmdStr = "SELECT COUNT(*) AS NumOfEmptySetionId"
                           + " FROM " + sectionTimes + " AS STI"
                           + " WHERE STI.sectionId is null"
                           + ";";
            dataGridView1.Enabled = true;
            lbDbName.Text = sSelectedDbName;
            UpdateDataGrid(connStr, cmdStr);
        }

        private void btnCheckEmptySectionId2_Click(object sender, EventArgs e)
        {
            string connStr = GetConnectionStr();
            string termPostfix = "_" + sSelectedDbName;
            string sectionLocations = " SectionLocations" + termPostfix;
            string cmdStr = "SELECT COUNT(*) AS NumOfEmptySetionId"
                           + " FROM " + sectionLocations + " AS SL"
                           + " WHERE SL.sectionId is null"
                           + ";";
            dataGridView1.Enabled = true;
            lbDbName.Text = sSelectedDbName;
            
            UpdateDataGrid(connStr, cmdStr);
        }

    

        private void checkBoxSectionEmptyCol_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSectionEmptyCol.Checked == true)
            {
                checkBoxSectionMissedFlg.Checked = false;
                cmbBoxSectionEmptyColumn.Enabled = true;            
            }
            else
            {
                cmbBoxSectionEmptyColumn.SelectedIndex = 0;
                cmbBoxSectionEmptyColumn.Enabled = false;
            }
                



        }

        private void checkBoxSectionMissedFlg_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSectionMissedFlg.Checked == true)
            {
                checkBoxSectionEmptyCol.Checked = false;
               
                cmbBoxSectionMissedFlg.Enabled = true;
            }
            else
            {
                cmbBoxSectionMissedFlg.SelectedIndex = 0;
                cmbBoxSectionMissedFlg.Enabled = false;
            }

        }

        private void cmbBoxSectionEmptyColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkPointNum = cmbBoxSectionEmptyColumn.SelectedIndex;           
        }

        private void cmbBoxSectionMissedFlg_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            checkPointNum = cmbBoxSectionMissedFlg.SelectedIndex+6;
        }



        private void btnSectionQuery_Click(object sender, EventArgs e)
        {
            switch (checkPointNum)
            {
                case Constants.EmptyCRN:
                    {
                        string connStr = GetConnectionStr();
                        string termPostfix = "_" + sSelectedDbName;
                        string sections = " Sections" + termPostfix;
                        string cmdStr = "SELECT COUNT(*) AS NumOfEmptyCRN"
                                        + " FROM " + sections + " AS S"
                                        + " WHERE S.CRN is null"
                                        + ";";
                        dataGridView1.Enabled = true;
                        lbDbName.Text = sSelectedDbName;
                        UpdateDataGrid(connStr, cmdStr);
                    }
                    break;
                case Constants.EmptyCRNLink:
                    {
                        string connStr = GetConnectionStr();
                        string termPostfix = "_" + sSelectedDbName;
                        string sections = " Sections" + termPostfix;
                        string cmdStr = "SELECT COUNT(*) AS NumOfEmptyCRNLink"
                                        + " FROM " + sections + " AS S"
                                        + " WHERE S.CRNLink is null"
                                        + ";";
                        dataGridView1.Enabled = true;
                        lbDbName.Text = sSelectedDbName;
                        UpdateDataGrid(connStr, cmdStr);
                    }
                    break;
                case Constants.EmptyCourseId:
                    {
                        string connStr = GetConnectionStr();
                        string termPostfix = "_" + sSelectedDbName;
                        string sections = " Sections" + termPostfix;
                        string cmdStr = "SELECT COUNT(*) AS NumOfEmptyCourseId"
                                        + " FROM " + sections + " AS S"
                                        + " WHERE S.courseId is null"
                                        + ";";
                        dataGridView1.Enabled = true;
                        lbDbName.Text = sSelectedDbName;
                        
                        UpdateDataGrid(connStr, cmdStr);
                    }
                    break;
                case Constants.EmptyScheduleTypeId:
                    {
                        string connStr = GetConnectionStr();
                        string termPostfix = "_" + sSelectedDbName;
                        string sections = " Sections" + termPostfix;
                        string cmdStr = "SELECT COUNT(*) AS NumOfEmptyScheduleId"
                                        + " FROM " + sections + " AS S"
                                        + " WHERE S.scheduleTypeId is null"
                                        + ";";
                        dataGridView1.Enabled = true;
                        lbDbName.Text = sSelectedDbName;
                        
                        UpdateDataGrid(connStr, cmdStr);
                    }
                    break;
                case Constants.EmptyTextBookLink:
                    {
                        string connStr = GetConnectionStr();
                        string termPostfix = "_" + sSelectedDbName;
                        string sections = " Sections" + termPostfix;
                        string cmdStr = "SELECT COUNT(*) AS NumOfEmptyTxtBookLink"
                                        + " FROM " + sections + " AS S"
                                        + " WHERE S.textBookLink is null"
                                        + ";";
                        dataGridView1.Enabled = true;
                        lbDbName.Text = sSelectedDbName;
                        
                        UpdateDataGrid(connStr, cmdStr);
                    }
                    break;
                case Constants.MissedIsEXL:
                    {
                        if (sSelectedDbName.Contains("2006") || sSelectedDbName.Contains("2007"))
                        {
                            MessageBox.Show("There is no EXL progmam in this year");
                            break;
                        }
                        string connStr = GetConnectionStr();
                        string termPostfix = "_" + sSelectedDbName;
                        string sections = " Sections" + termPostfix;
                        string cmdStr = "SELECT COUNT(*) AS NumOfIsEXL"
                                        + " FROM " + sections + " AS S"
                                        + " WHERE S.isEXL = 1"
                                        + ";";                        
                        dataGridView1.Enabled = true;
                        lbDbName.Text = sSelectedDbName;
                        
                        UpdateDataGrid(connStr, cmdStr);
                    }
                    break;
                case Constants.MissedIsTransferIN:
                        {                            
                            string connStr = GetConnectionStr();
                            string termPostfix = "_" + sSelectedDbName;
                            string sections = " Sections" + termPostfix;
                            string cmdStr = "SELECT COUNT(*) AS NumOfIsTransferIN"
                                            + " FROM " + sections + " AS S"
                                            + " WHERE S.isTransferIN = 1"
                                            + ";";
                            dataGridView1.Enabled = true;
                            lbDbName.Text = sSelectedDbName;
                            
                            UpdateDataGrid(connStr, cmdStr);
                        }
                        break;
                case Constants.MissedIsETIE:
                        {
                            if (sSelectedDbName.Contains("2006") || sSelectedDbName.Contains("2007") || sSelectedDbName.Contains("2008") || sSelectedDbName.Contains("2009"))
                            {
                                MessageBox.Show("There is no ETIE progmam in this year");
                                break;
                            }
                            string connStr = GetConnectionStr();
                            string termPostfix = "_" + sSelectedDbName;
                            string sections = " Sections" + termPostfix;
                            string cmdStr = "SELECT COUNT(*) AS NumOfIsETIE"
                                            + " FROM " + sections + " AS S"
                                            + " WHERE S.isETIE= 1"
                                            + ";";
                            dataGridView1.Enabled = true;
                            lbDbName.Text = sSelectedDbName;
                            
                            UpdateDataGrid(connStr, cmdStr);
                        }
                        break;
                case Constants.MissedIsCanceled:
                        {
                           
                            string connStr = GetConnectionStr();
                            string termPostfix = "_" + sSelectedDbName;
                            string sections = " Sections" + termPostfix;
                            string cmdStr = "SELECT COUNT(*) AS NumOfIsCanceled"
                                            + " FROM " + sections + " AS S"
                                            + " WHERE S.isCanceled = 1"
                                            + ";";
                            dataGridView1.Enabled = true;
                            lbDbName.Text = sSelectedDbName;
                            
                            UpdateDataGrid(connStr, cmdStr);
                        }
                        break;
                case Constants.MissedIsOnline:
                        {
                            string connStr = GetConnectionStr();
                            string termPostfix = "_" + sSelectedDbName;
                            string sections = " Sections" + termPostfix;
                            string cmdStr = "SELECT COUNT(*) AS NumOfMissedIsOnline"
                                            + " FROM " + sections + " AS S"
                                            + " WHERE S.isOnline = 0"
                                            + " AND S.notes like 'Distance Learning'"
                                            + ";";
                            dataGridView1.Enabled = true;
                            lbDbName.Text = sSelectedDbName;
                            
                            UpdateDataGrid(connStr, cmdStr);
                        }
                        break;
                case Constants.MissedIsSIA:
                        {
                            if (!sSelectedDbName.Contains("2013") && !sSelectedDbName.Contains("2014") && !sSelectedDbName.Contains("2015"))
                            {
                                MessageBox.Show("There is no Supplemental Instruction Available in this year");
                                break;
                            }
                            string connStr = GetConnectionStr();
                            string termPostfix = "_" + sSelectedDbName;
                            string sections = " Sections" + termPostfix;
                            string cmdStr = "SELECT COUNT(*) AS NumOfIsSIA"
                                            + " FROM " + sections + " AS S"
                                            + " WHERE S.isSIA= 1"
                                            + ";";
                            dataGridView1.Enabled = true;
                            lbDbName.Text = sSelectedDbName;
                            
                            UpdateDataGrid(connStr, cmdStr);
                        }
                        break;
               
                default:

                    break;
            }
        }

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "Custom Description"; //not mandatory

            if (fbd.ShowDialog() == DialogResult.OK)
                sSelectedFolder = fbd.SelectedPath;
            else
                sSelectedFolder = string.Empty;

            txtDirectory1.Text = sSelectedFolder;

            clBoxSqls.Items.Clear();

            if (Directory.Exists(txtDirectory1.Text))
            {

                string[] fileEntries = ProcessDirectory(txtDirectory1.Text);

                 
                foreach (string fileName in fileEntries)
                {
                    string extension;

                    extension = Path.GetExtension(fileName);
                    if (extension == ".sql")
                    {
                        string dbName;
                        dbName = Path.GetFileNameWithoutExtension(fileName);
                        clBoxSqls.Items.Add(dbName);
                    }

                }

                // Allow the ListBox to repaint and display the new items.
                //listBoxDBsTab1.EndUpdate();
            }
            else
            {
                MessageBox.Show(txtDirectory1.Text + " is not a valid directory.");
            }
        }

        private void btnDebugQuery_Click(object sender, EventArgs e)
        {

        }

        private void cmbSelectTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dataName = cmbSelectTerm.SelectedItem.ToString();
            if (dataName != "Terms")
            {
                quarter = Regex.Replace(dataName, @"\d", "");
                string currentYearStr = Regex.Replace(dataName, @"\D", "");
                year = Convert.ToInt16(currentYearStr);
                tb_output.Text = year.ToString();
            }
            else
            {
                quarter = "N/A";
                year = Convert.ToInt16("1990");
                tb_output.Text = "Select a term first";
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "Custom Description"; //not mandatory

            if (fbd.ShowDialog() == DialogResult.OK)
                sSelectedFolder = fbd.SelectedPath;
            else
                sSelectedFolder = string.Empty;

            txtDirectory2.Text = sSelectedFolder;



            if (Directory.Exists(txtDirectory2.Text))
            {

                string[] fileEntries = ProcessDirectory(txtDirectory2.Text);
                cmbSelectTerm.Items.Clear();
                cmbSelectTerm.Items.Add("Terms");
                //listBoxDBsTab1.BeginUpdate();
                // Loop through and add 50 items to the ListBox. 
                foreach (string fileName in fileEntries)
                {
                    string extension;

                    extension = Path.GetExtension(fileName);
                    if (extension == ".htm")
                    {
                        string dataName;
                        dataName = Path.GetFileNameWithoutExtension(fileName);
                        cmbSelectTerm.Items.Add(dataName);
                    }

                }

                // Allow the ListBox to repaint and display the new items.
                //listBoxDBsTab1.EndUpdate();
            }
            else
            {
                MessageBox.Show(txtDirectory2.Text + " is not a valid directory.");
            }
        }

        private void PrepareNumberExtra(string numberExtra)
        {
            number = numberExtra.Replace(System.Environment.NewLine, "");
            string[] words;
            words = number.Split(new string[] { "(" }, StringSplitOptions.None);
            number = words[0].Trim().Replace(System.Environment.NewLine, "");
            words[1] = words[1].Trim().Replace(")", "");
            switch (words[1])
            {
                case "TransferIN":
                    isTransfer = "1";
                    break;
                case "EXL":
                    isEXL = "1";
                    break;
            }
            //tb_output.AppendText("TransferIn: Yes" + System.Environment.NewLine);
        }
        private void PrepareTimes(string times)
        {
            if (times.Contains("-"))
            {
                string[] words;
                words = times.Split(new string[] { "-" }, StringSplitOptions.None);
                startTime = words[0].Trim();
                endTime = words[1].Trim();
            }
            else
            {
                startTime = times.Trim();
                endTime = "";

            }
        }
        private void PrepareSectionDates(string SectionDates)
        {
            string[] words;
            words = SectionDates.Split(new string[] { "to" }, StringSplitOptions.None);
            startDate = words[0].Trim().Replace(System.Environment.NewLine, "");
            endDate = words[1].Trim().Replace(System.Environment.NewLine, "");
        }

        private void PrepareWaitlist(string waitList)
        {
            string[] part = waitList.Trim().Split(new string[] { "/" }, StringSplitOptions.None);
            if (part[0].Equals("N")) { waitTaken = "0"; } else { waitTaken = part[0].ToString(); }
            if (part[1].Equals("A")) { waitAvail = "0"; } else { waitAvail = part[1].ToString(); }
        }

        private void PreparePreReq(string PreReq)
        {
            PreReq = PreReq.Replace(System.Environment.NewLine, "");
            string[] words;
            words = PreReq.Split(new string[] { "and", "or" }, StringSplitOptions.None);
            foreach (string req in words)
            {
                PreReqList.Add(req.Trim());
            }
        }

        private void PrepareCoReq(string CoReq)
        {
            CoReq = CoReq.Replace(System.Environment.NewLine, "");
            string[] words;
            words = CoReq.Split(new string[] { "and", "or" }, StringSplitOptions.None);
            foreach (string req in words)
            {
                CoReqList.Add(req.Trim());
            }
        }

        private void InsertCourseIntoDatabase(SQLiteConnection conn)
        {
            /*Insert Subject*/
            if (!subject_list.Contains("++" + subject))
            {
                subject_list = subject_list + "++" + subject;
                SQLiteCommand cmd_insert_instructor = new SQLiteCommand(conn);
                SQLiteParameter paramSubject = new SQLiteParameter();
                cmd_insert_instructor.CommandText = "INSERT INTO Subjects(subject) SELECT ? WHERE NOT EXISTS(SELECT 1 FROM Subjects WHERE subject = ?)";
                paramSubject.DbType = DbType.String;
                paramSubject.Value = subject;
                cmd_insert_instructor.Parameters.Add(paramSubject);
                cmd_insert_instructor.Parameters.Add(paramSubject);

                cmd_insert_instructor.ExecuteNonQuery();
            }

            if (!course_list.Contains("++" + subject + number)) //Checks if the Course Number already exists
            {
                /*Insert Course*/
                SQLiteCommand cmd_insert_course = new SQLiteCommand(conn);
                SQLiteParameter paramSubjectId = new SQLiteParameter();
                SQLiteParameter paramcourseNum = new SQLiteParameter();
                SQLiteParameter paramCredits = new SQLiteParameter();
                cmd_insert_course.CommandText = "INSERT INTO Courses (subjectid, courseNum, credits) VALUES ((SELECT subjectid FROM Subjects WHERE subject = ?), ?, ?); select last_insert_rowid()";
                paramSubjectId.DbType = DbType.String;
                paramSubjectId.Value = subject;
                paramcourseNum.DbType = DbType.String;
                paramcourseNum.Value = number;
                paramCredits.DbType = DbType.String;
                paramCredits.Value = crHrs;
                cmd_insert_course.Parameters.Add(paramSubjectId);
                cmd_insert_course.Parameters.Add(paramcourseNum);
                cmd_insert_course.Parameters.Add(paramCredits);

                courseID = (Int64)cmd_insert_course.ExecuteScalar();
                course_list = course_list + "++" + subject + number;
            }
            else
            {
                SQLiteCommand cmd_insert_course = new SQLiteCommand(conn);
                SQLiteParameter paramCourseId = new SQLiteParameter();
                cmd_insert_course.CommandText = "SELECT courseId FROM Courses WHERE courseNum = ?";
                paramCourseId.DbType = DbType.String;
                paramCourseId.Value = number;
                cmd_insert_course.Parameters.Add(paramCourseId);
                courseID = (Int64)cmd_insert_course.ExecuteScalar();
            }
        }

        private void InsertSectionIntoDatabase(SQLiteConnection conn)
        {
            /*Insert Instructor*/
            SQLiteCommand cmd_insert_instructor = new SQLiteCommand(conn);
            cmd_insert_instructor.CommandText = "INSERT INTO Instructors(firstName,lastName) SELECT ?, ? WHERE NOT EXISTS(SELECT 1 FROM Instructors WHERE firstName = ? AND lastName = ?)";
            SQLiteParameter paramfirstName = new SQLiteParameter();
            SQLiteParameter paramlastName = new SQLiteParameter();
            if (instructor.Equals("To Be Announced"))
            {
                paramfirstName.DbType = DbType.String;
                firstName = "To Be Announced";
                paramfirstName.Value = firstName;
                paramlastName.DbType = DbType.String;
                lastName = "To Be Announced";
                paramlastName.Value = lastName;

            }
            else
            {
                string[] words;
                words = instructor.Split(new string[] { "," }, StringSplitOptions.None);
                paramfirstName.DbType = DbType.String;
                firstName = words[1].Trim();
                paramfirstName.Value = firstName;
                paramlastName.DbType = DbType.String;
                lastName = words[0].Trim();
                paramlastName.Value = lastName;
            }
            if (!instructor_list.Contains("++" + firstName + "," + lastName))
            {
                instructor_list = instructor_list + "++" + firstName + "," + lastName;
                cmd_insert_instructor.Parameters.Add(paramfirstName);
                cmd_insert_instructor.Parameters.Add(paramlastName);
                cmd_insert_instructor.Parameters.Add(paramfirstName);
                cmd_insert_instructor.Parameters.Add(paramlastName);

                cmd_insert_instructor.ExecuteNonQuery();
            }


            /*Insert Schedule Type*/
            if (!schedule_list.Contains("++" + subject))
            {
                subject_list = subject_list + "++" + subject;
                SQLiteCommand cmd_insert_type = new SQLiteCommand(conn);
                SQLiteParameter paramType = new SQLiteParameter();
                cmd_insert_type.CommandText = "INSERT INTO ScheduleTypes(scheduleType) SELECT ? WHERE NOT EXISTS(SELECT 1 FROM ScheduleTypes WHERE scheduleType = ?);";
                paramType.DbType = DbType.String;
                paramType.Value = schedule;
                cmd_insert_type.Parameters.Add(paramType);
                cmd_insert_type.Parameters.Add(paramType);

                cmd_insert_type.ExecuteNonQuery();
            }

            /*Insert Building*/
            if (building != string.Empty)
            {
                building = building.Replace(System.Environment.NewLine, "");
                SQLiteCommand cmd_insert_building = new SQLiteCommand(conn);
                SQLiteParameter paramBuilding = new SQLiteParameter();
                cmd_insert_building.CommandText = "INSERT INTO Buildings(building) SELECT ? WHERE NOT EXISTS(SELECT 1 FROM Buildings WHERE building = ?)";
                if (building.Contains("-"))
                {
                    string[] words;
                    words = building.Split(new string[] { "-" }, StringSplitOptions.None);
                    building = words[0].Trim();
                    roomNumber = words[1].Trim();
                    paramBuilding.DbType = DbType.String;
                    paramBuilding.Value = words[0].Trim();
                }
                else
                {
                    paramBuilding.DbType = DbType.String;
                    paramBuilding.Value = building.Trim();
                }
                if (!building_list.Contains("++" + building))
                {
                    building_list = building_list + "++" + building;
                    cmd_insert_building.Parameters.Add(paramBuilding);
                    cmd_insert_building.Parameters.Add(paramBuilding);

                    cmd_insert_building.ExecuteNonQuery();
                }

            }

            if (sectionID == -1)
            {
                /*Insert Section*/
                SQLiteCommand cmd_insert_section = new SQLiteCommand(conn);
                SQLiteParameter paramTermYear = new SQLiteParameter();
                SQLiteParameter paramTermQuarter = new SQLiteParameter();
                SQLiteParameter paramCRN = new SQLiteParameter();
                SQLiteParameter paramCourseID = new SQLiteParameter();
                SQLiteParameter paramNumber = new SQLiteParameter();
                SQLiteParameter paramTitle = new SQLiteParameter();
                SQLiteParameter paramSchedule = new SQLiteParameter();
                SQLiteParameter paramInstructorFn = new SQLiteParameter();
                SQLiteParameter paramInstructorLn = new SQLiteParameter();
                SQLiteParameter paramMeetingStart = new SQLiteParameter();
                SQLiteParameter paramMeetingEnd = new SQLiteParameter();
                SQLiteParameter paramEnrlCap = new SQLiteParameter();
                SQLiteParameter paramEnrlAct = new SQLiteParameter();
                SQLiteParameter paramWaitCap = new SQLiteParameter();
                SQLiteParameter paramWaitAct = new SQLiteParameter();
                SQLiteParameter paramIsExl = new SQLiteParameter();
                SQLiteParameter paramIsEtie = new SQLiteParameter();
                SQLiteParameter paramIsTransferIn = new SQLiteParameter();
                SQLiteParameter paramIsCancelled = new SQLiteParameter();
                SQLiteParameter paramIsOnline = new SQLiteParameter();
                SQLiteParameter paramIsSia = new SQLiteParameter();
                SQLiteParameter paramTextBookLink = new SQLiteParameter();
                SQLiteParameter paramCRNLink = new SQLiteParameter();
                SQLiteParameter paramNotes = new SQLiteParameter();
                paramTermYear.DbType = DbType.Int16;
                paramTermQuarter.DbType = DbType.String;
                paramCRN.DbType = DbType.String;
                paramCourseID.DbType = DbType.Int16;
                paramNumber.DbType = DbType.String;
                paramTitle.DbType = DbType.String;
                paramSchedule.DbType = DbType.String;
                paramInstructorFn.DbType = DbType.String;
                paramInstructorLn.DbType = DbType.String;
                paramMeetingStart.DbType = DbType.String;
                paramMeetingEnd.DbType = DbType.String;
                paramEnrlCap.DbType = DbType.Int16;
                paramEnrlAct.DbType = DbType.Int16;
                paramWaitCap.DbType = DbType.Int16;
                paramWaitAct.DbType = DbType.Int16;
                paramIsExl.DbType = DbType.Int16;
                paramIsEtie.DbType = DbType.Int16;
                paramIsTransferIn.DbType = DbType.Int16;
                paramIsCancelled.DbType = DbType.Int16;
                paramIsOnline.DbType = DbType.Int16;
                paramIsSia.DbType = DbType.Int16;
                paramTextBookLink.DbType = DbType.String;
                paramCRNLink.DbType = DbType.String;
                paramNotes.DbType = DbType.String;
                cmd_insert_section.CommandText = "INSERT INTO Sections (termId, CRN, courseId, sectionNum, title, scheduleTypeId, instructorId, meetingStart, meetingEnd, enrlCap, enrlAct, waitCap, waitAct, isEXL, isETIE, isTransferIN, isCanceled, isOnline, isSIA, textBookLink, CRNLink, notes) VALUES ((SELECT termId FROM Terms WHERE quarter = ? AND year = ?), ?, ?,?,?,(SELECT scheduleTypeId FROM ScheduleTypes WHERE scheduleType = ?), (SELECT instructorId FROM Instructors WHERE firstName = ? AND lastName = ?),?,?,?,?,?,?,?,?,?,?,?,?,?,?,?); select last_insert_rowid()";
                paramTermQuarter.Value = quarter;
                paramTermYear.Value = year;
                paramCRN.Value = CRN;
                paramCourseID.Value = courseID;
                paramNumber.Value = section;
                paramTitle.Value = title;
                paramSchedule.Value = schedule;
                paramInstructorFn.Value = firstName;
                paramInstructorLn.Value = lastName;
                paramMeetingStart.Value = startDate;
                paramMeetingEnd.Value = endDate;
                paramEnrlCap.Value = int.Parse(enrAvail);
                paramEnrlAct.Value = int.Parse(enrTaken);
                paramWaitCap.Value = int.Parse(waitAvail);
                paramWaitAct.Value = int.Parse(waitTaken);
                paramIsExl.Value = int.Parse(isEXL);
                paramIsEtie.Value = int.Parse(isETIE);
                paramIsTransferIn.Value = int.Parse(isTransfer);
                paramIsCancelled.Value = int.Parse(isCancelled);
                paramIsOnline.Value = int.Parse(isOnline);
                paramIsSia.Value = int.Parse(isSIA);
                paramTextBookLink.Value = textbookLink;
                paramCRNLink.Value = CRNLink;
                paramNotes.Value = notes;

                cmd_insert_section.Parameters.Add(paramTermQuarter);
                cmd_insert_section.Parameters.Add(paramTermYear);
                cmd_insert_section.Parameters.Add(paramCRN);
                cmd_insert_section.Parameters.Add(paramCourseID);
                cmd_insert_section.Parameters.Add(paramNumber);
                cmd_insert_section.Parameters.Add(paramTitle);
                cmd_insert_section.Parameters.Add(paramSchedule);
                cmd_insert_section.Parameters.Add(paramfirstName);
                cmd_insert_section.Parameters.Add(paramlastName);
                cmd_insert_section.Parameters.Add(paramMeetingStart);
                cmd_insert_section.Parameters.Add(paramMeetingEnd);
                cmd_insert_section.Parameters.Add(paramEnrlCap);
                cmd_insert_section.Parameters.Add(paramEnrlAct);
                cmd_insert_section.Parameters.Add(paramWaitCap);
                cmd_insert_section.Parameters.Add(paramWaitAct);
                cmd_insert_section.Parameters.Add(paramIsExl);
                cmd_insert_section.Parameters.Add(paramIsEtie);
                cmd_insert_section.Parameters.Add(paramIsTransferIn);
                cmd_insert_section.Parameters.Add(paramIsCancelled);
                cmd_insert_section.Parameters.Add(paramIsOnline);
                cmd_insert_section.Parameters.Add(paramIsSia);
                cmd_insert_section.Parameters.Add(paramTextBookLink);
                cmd_insert_section.Parameters.Add(paramCRNLink);
                cmd_insert_section.Parameters.Add(paramNotes);


                sectionID = (Int64)cmd_insert_section.ExecuteScalar();

            }

            /*Insert Section Time*/
            SQLiteCommand cmd_insert_section_time = new SQLiteCommand(conn);
            SQLiteParameter paramSectionID_time = new SQLiteParameter();
            SQLiteParameter paramDay = new SQLiteParameter();
            SQLiteParameter paramTimeStart = new SQLiteParameter();
            SQLiteParameter paramTimeEnd = new SQLiteParameter();
            paramSectionID_time.DbType = DbType.Int64;
            paramDay.DbType = DbType.String;
            paramTimeStart.DbType = DbType.String;
            paramTimeEnd.DbType = DbType.String;
            cmd_insert_section_time.CommandText = "INSERT INTO SectionTimes (sectionId, day, timeStart, timeEnd) VALUES (?, ?, ?, ?);";
            paramSectionID_time.Value = sectionID;
            paramDay.Value = days;
            paramTimeStart.Value = startTime;
            paramTimeEnd.Value = endTime;
            cmd_insert_section_time.Parameters.Add(paramSectionID_time);
            cmd_insert_section_time.Parameters.Add(paramDay);
            cmd_insert_section_time.Parameters.Add(paramTimeStart);
            cmd_insert_section_time.Parameters.Add(paramTimeEnd);


            cmd_insert_section_time.ExecuteNonQuery();


            /*Insert Section Locations*/
            SQLiteCommand cmd_insert_section_location = new SQLiteCommand(conn);
            SQLiteParameter paramSectionID_loc = new SQLiteParameter();
            SQLiteParameter paramRoomNum = new SQLiteParameter();
            SQLiteParameter paramBuilding_loc = new SQLiteParameter();
            paramSectionID_loc.DbType = DbType.Int64;
            paramRoomNum.DbType = DbType.String;
            paramBuilding_loc.DbType = DbType.String;
            cmd_insert_section_location.CommandText = "INSERT INTO SectionLocations (sectionId, roomNum, buildingId) VALUES (?, ?, (SELECT buildingId FROM Buildings WHERE building = ?));";
            paramSectionID_loc.Value = sectionID;
            paramRoomNum.Value = roomNumber;
            paramBuilding_loc.Value = building;
            cmd_insert_section_location.Parameters.Add(paramSectionID_loc);
            cmd_insert_section_location.Parameters.Add(paramRoomNum);
            cmd_insert_section_location.Parameters.Add(paramBuilding_loc);


            cmd_insert_section_location.ExecuteNonQuery();


            /*Insert PreRequisites*/
            if (PreReqList.Count > 0)
            {
                foreach (string PreReqSingle in PreReqList)
                {

                    string[] words;
                    words = PreReqSingle.Split(new string[] { " " }, StringSplitOptions.None);

                    SQLiteCommand cmd_insert_PreReq = new SQLiteCommand(conn);
                    SQLiteParameter paramSectionID_Pre = new SQLiteParameter();
                    SQLiteParameter paramSubject_Pre = new SQLiteParameter();
                    SQLiteParameter paramCourseNum_Pre = new SQLiteParameter();
                    paramSectionID_Pre.DbType = DbType.Int64;
                    paramSubject_Pre.DbType = DbType.String;
                    paramCourseNum_Pre.DbType = DbType.String;
                    cmd_insert_PreReq.CommandText = "INSERT INTO PreRequisites (sectionId, subject, courseNum, prCourseId) VALUES (?, ?, ?, (SELECT courseId FROM Courses WHERE subjectId = (SELECT subjectId FROM Subjects WHERE subject = ?) AND courseNum = ?));";
                    paramSectionID_Pre.Value = sectionID;
                    paramSubject_Pre.Value = words[0].Trim();
                    paramCourseNum_Pre.Value = words[1].Trim();
                    cmd_insert_PreReq.Parameters.Add(paramSectionID_Pre);
                    cmd_insert_PreReq.Parameters.Add(paramSubject_Pre);
                    cmd_insert_PreReq.Parameters.Add(paramCourseNum_Pre);
                    cmd_insert_PreReq.Parameters.Add(paramSubject_Pre);
                    cmd_insert_PreReq.Parameters.Add(paramCourseNum_Pre);

                    cmd_insert_PreReq.ExecuteNonQuery();

                }
                PreReqList.Clear();
            }

            /*Insert CoRequisites*/
            if (CoReqList.Count > 0)
            {
                foreach (string CoReqSingle in CoReqList)
                {

                    string[] words;
                    words = CoReqSingle.Split(new string[] { " " }, StringSplitOptions.None);

                    SQLiteCommand cmd_insert_CoReq = new SQLiteCommand(conn);
                    SQLiteParameter paramSectionID_Co = new SQLiteParameter();
                    SQLiteParameter paramSubject_Co = new SQLiteParameter();
                    SQLiteParameter paramCourseNum_Co = new SQLiteParameter();
                    paramSectionID_Co.DbType = DbType.Int64;
                    paramSubject_Co.DbType = DbType.String;
                    paramCourseNum_Co.DbType = DbType.String;
                    cmd_insert_CoReq.CommandText = "INSERT INTO CoRequisites (sectionId, subject, courseNum, crCourseId) VALUES (?, ?, ?, (SELECT courseId FROM Courses WHERE subjectId = (SELECT subjectId FROM Subjects WHERE subject = ?) AND courseNum = ?));";
                    paramSectionID_Co.Value = sectionID;
                    paramSubject_Co.Value = words[0].Trim();
                    paramCourseNum_Co.Value = words[1].Trim();
                    cmd_insert_CoReq.Parameters.Add(paramSectionID_Co);
                    cmd_insert_CoReq.Parameters.Add(paramSubject_Co);
                    cmd_insert_CoReq.Parameters.Add(paramCourseNum_Co);
                    cmd_insert_CoReq.Parameters.Add(paramSubject_Co);
                    cmd_insert_CoReq.Parameters.Add(paramCourseNum_Co);

                    cmd_insert_CoReq.ExecuteNonQuery();
                }
                CoReqList.Clear();
            }
        }

        private void btnParseData_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            toolStripLabel.Text = "Running";
            statusStrip.Refresh();

            SQLiteConnection conn = new SQLiteConnection();
            conn.ConnectionString = connString;

            /*Insert New Term*/
            //SQLiteCommand cmd_insert_term = new SQLiteCommand(conn);
            //SQLiteParameter paramQuarter = new SQLiteParameter();
            //SQLiteParameter paramYear = new SQLiteParameter();
            //cmd_insert_term.CommandText = "INSERT INTO Terms(quarter, year) SELECT ? , ? WHERE NOT EXISTS(SELECT 1 FROM Terms WHERE quarter= ? AND year = ?)";
            //paramQuarter.DbType = DbType.String;
            //paramQuarter.Value = quarter;
            //paramYear.DbType = DbType.Int16;
            //paramYear.Value = year;
            //cmd_insert_term.Parameters.Add(paramQuarter);
            //cmd_insert_term.Parameters.Add(paramYear);
            //cmd_insert_term.Parameters.Add(paramQuarter);
            //cmd_insert_term.Parameters.Add(paramYear);
            conn.Open();
            //cmd_insert_term.ExecuteNonQuery();
            /*Finish Insert*/

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            // doc.Load("../../2009 Fall - On-Line Class Schedule Query Results_original.htm");
            string dataPath = txtDirectory1.Text + Path.DirectorySeparatorChar + cmbSelectTerm.SelectedItem + ".htm";
            doc.Load(dataPath);
            HtmlNode base_table = doc.DocumentNode.SelectSingleNode("//table [@class='datadisplaytable']");
            foreach (HtmlNode row in base_table.ChildNodes)
            {
                tb_output.Text = row.ToString();
                if (row.InnerHtml.Equals(System.Environment.NewLine) || row.OuterHtml.Contains("ddheader")) { }
                else if (row.ChildNodes.Count == 0)
                {
                }
                else
                {


                    switch (row.ChildNodes[1].ChildNodes.Count)
                    {
                        case 19: //Normal first row will have 19 children. This includes returns and data
                            subject = "";
                            CRN = "";
                            title = "";
                            startDate = "";
                            endDate = "";
                            number = "";
                            isTransfer = "0";
                            isEXL = "0";
                            isETIE = "0";
                            isSIA = "0";
                            section = "";
                            crHrs = "";
                            enrAvail = "";
                            enrTaken = "";
                            sectionID = -1;
                            CRN = row.ChildNodes[1].ChildNodes[1].InnerText.Trim()
                                .Replace(System.Environment.NewLine, "");
                            CRNLink =
                                row.ChildNodes[1].ChildNodes[1].Attributes["href"].Value.Replace(
                                    "javascript:openWindow('", "").Replace("')", "").Trim();
                            subject =
                                row.ChildNodes[1].ChildNodes[3].InnerText.Trim().Replace(System.Environment.NewLine, "");
                            number = row.ChildNodes[1].ChildNodes[5].InnerText.Trim()
                                .Replace(System.Environment.NewLine, "");
                            section =
                                row.ChildNodes[1].ChildNodes[7].InnerText.Trim().Replace(System.Environment.NewLine, "");
                            crHrs = row.ChildNodes[1].ChildNodes[9].InnerText.Trim()
                                .Replace(System.Environment.NewLine, "");
                            enrTaken = row.ChildNodes[1].ChildNodes[11].ChildNodes[0].InnerText.Replace("/", "").Trim();
                            enrAvail = row.ChildNodes[1].ChildNodes[11].ChildNodes[1].InnerText.Replace("/", "").Trim();
                            PrepareWaitlist(row.ChildNodes[1].ChildNodes[13].InnerText);
                            title = row.ChildNodes[1].ChildNodes[15].InnerText.Trim()
                                .Replace(System.Environment.NewLine, "");
                            PrepareSectionDates(row.ChildNodes[1].ChildNodes[17].InnerText);
                            InsertCourseIntoDatabase(conn);
                            break;
                        case 6:
                            //Non-Normal first rows will have 6 children. The paranthesis in the # column messes up the counting. 
                            subject = "";
                            CRN = "";
                            title = "";
                            startDate = "";
                            endDate = "";
                            number = "";
                            isTransfer = "0";
                            isEXL = "0";
                            isETIE = "0";
                            isSIA = "0";
                            section = "";
                            crHrs = "";
                            enrTaken = "";
                            enrAvail = "";
                            sectionID = -1;
                            roomNumber = "";
                            CRN = row.ChildNodes[1].ChildNodes[1].InnerText.Trim()
                                .Replace(System.Environment.NewLine, "");
                            subject =
                                row.ChildNodes[1].ChildNodes[3].InnerText.Trim().Replace(System.Environment.NewLine, "");
                            PrepareNumberExtra(row.ChildNodes[1].ChildNodes[5].ChildNodes[1].InnerText);
                            //Needed to fix paranthesis issue. Has to go one more child deeper
                            section =
                                row.ChildNodes[1].ChildNodes[5].ChildNodes[3].InnerText.Trim()
                                    .Replace(System.Environment.NewLine, "");
                            crHrs =
                                row.ChildNodes[1].ChildNodes[5].ChildNodes[5].InnerText.Trim()
                                    .Replace(System.Environment.NewLine, "");
                            enrTaken =
                                row.ChildNodes[1].ChildNodes[5].ChildNodes[7].ChildNodes[0].InnerText.Replace("/", "")
                                    .Trim();
                            enrAvail =
                                row.ChildNodes[1].ChildNodes[5].ChildNodes[7].ChildNodes[1].InnerText.Replace("/", "")
                                    .Trim();
                            PrepareWaitlist(row.ChildNodes[1].ChildNodes[5].ChildNodes[9].InnerText);
                            title =
                                row.ChildNodes[1].ChildNodes[5].ChildNodes[11].InnerText.Trim()
                                    .Replace(System.Environment.NewLine, "");
                            PrepareSectionDates(row.ChildNodes[1].ChildNodes[5].ChildNodes[13].InnerText);
                            InsertCourseIntoDatabase(conn);
                            break;
                        default: //For non first row children
                            schedule = "";
                            instructor = "";
                            building = "";
                            startTime = "";
                            endTime = "";
                            isOnline = "0";
                            isCancelled = "0";
                            textbookLink = "";
                            notes = "";
                            sectionID = -1;
                            schedule =
                                row.ChildNodes[1].ChildNodes[0].InnerText.Trim().Replace(System.Environment.NewLine, "");
                            if (schedule.Equals("Distance Learning"))
                            {
                                isOnline = "1";
                            }
                            days = row.ChildNodes[3].ChildNodes[0].InnerText.Trim();
                            PrepareTimes(row.ChildNodes[5].ChildNodes[0].InnerText.Trim());
                            building =
                                row.ChildNodes[7].ChildNodes[0].InnerText.Trim().Replace(System.Environment.NewLine, "");
                            instructor =
                                row.ChildNodes[9].ChildNodes[0].InnerText.Trim().Replace(System.Environment.NewLine, "");
                            if (row.ChildNodes.Count >= 12)
                            {
                                int skip = 0;
                                foreach (HtmlNode comment in row.ChildNodes[11].ChildNodes)
                                //The comment row can have numerous things. 
                                {
                                    if (skip == 0)
                                    {
                                        if (comment.InnerText.Equals("Pre-Requisites:"))
                                        {
                                            PreparePreReq(comment.NextSibling.InnerHtml.ToString().Trim());
                                            skip = 1;
                                        }
                                        else if (comment.InnerText.Equals("Co-Requisites:"))
                                        {
                                            PrepareCoReq(comment.NextSibling.InnerHtml.Trim());
                                            skip = 1;
                                        }
                                        else if (comment.InnerText.Contains("***CANCELED***"))
                                        {
                                            isCancelled = "1";
                                            skip = 0;
                                        }
                                        else if (comment.InnerText.Contains("View Books"))
                                        {
                                            textbookLink = comment.Attributes["href"].Value;
                                            skip = 0;
                                        }
                                        else
                                        {
                                            if (comment.InnerText.Trim() != string.Empty)
                                            {
                                                notes = comment.InnerText;
                                                skip = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        skip = 0;
                                        continue;
                                    }
                                }
                            }
                            InsertSectionIntoDatabase(conn);
                            break;
                    }
                }
            }
            conn.Close();
            toolStripLabel.Text = "Finished";
            statusStrip.Refresh();
            stopwatch.Stop();
            tb_output.AppendText(DateTime.Now.ToShortTimeString() + System.Environment.NewLine);
            tb_output.AppendText(stopwatch.Elapsed.ToString());
        }

        private void txtDirectory2_TextChanged(object sender, EventArgs e)
        {

        }
             
    }

    static class Constants
    {
        public const int EmptyCRN = 1;
        public const int EmptyCRNLink = 2;
        public const int EmptyCourseId = 3;
        public const int EmptyScheduleTypeId = 4;
        public const int EmptyTextBookLink = 5;

        public const int MissedIsEXL = 7;
        public const int MissedIsTransferIN = 8;
        public const int MissedIsETIE = 9;
        public const int MissedIsCanceled = 10;
        public const int MissedIsOnline = 11;
        public const int MissedIsSIA = 12;
        
    }
}
