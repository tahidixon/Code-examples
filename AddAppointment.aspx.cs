using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace Assignment3.Appointment_Manager
{
    public partial class AddAppointment : System.Web.UI.Page
    {
        
        SQL.HealthclinicEntities dbcontext = new SQL.HealthclinicEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            dbcontext.Hospitals.Load();
            if (!IsPostBack)
            {
                var result = from x in dbcontext.Hospitals.Local
                             select new
                             {
                                 Location = x.Name + ", " + x.Address + ", " + x.City,
                                 x.HospitalID
                             };
                // Add data to Location Dropdown List

                locationDropDownList.DataTextField = "Location";
                locationDropDownList.DataValueField = "HospitalID";
                locationDropDownList.DataSource = result;
                locationDropDownList.DataBind();
            }

        }

        protected void locationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            dbcontext.Hospitals.Load();
            

                SQL.Hospital selectedHospital =
                    (from hospital in dbcontext.Hospitals.Local
                     where hospital.HospitalID == Convert.ToInt32(locationDropDownList.SelectedValue)
                     select hospital).First();




                dbcontext.Departments.Load();


                IEnumerable<SQL.Department> deptList =
                    (from x in dbcontext.Departments
                     where x.HospitalID == selectedHospital.HospitalID
                     select x).AsEnumerable();


                /*
                foreach (SQL.Department dept in deptList)
                {

                }
                */

                var result = from x in deptList
                             select new
                             {
                                 x.Name,
                                 x.DepartmentID
                             };

                departmentDropDownList.DataTextField = "Name";
                departmentDropDownList.DataValueField = "DepartmentID";
                departmentDropDownList.DataSource = result;
                departmentDropDownList.DataBind();
         
        }

        protected void departmentDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            dbcontext.Departments.Load();

            SQL.Department selectedDepartment =
                (from department in dbcontext.Departments.Local
                 where department.DepartmentID == Convert.ToInt32(departmentDropDownList.SelectedValue)
                 select department).First();

            dbcontext.Doctors.Load();

            IEnumerable<SQL.Doctor> docList =
                (from x in dbcontext.Doctors
                 where x.DepartmentID == selectedDepartment.DepartmentID
                 select x).AsEnumerable();


            var result = from x in docList
                         select new
                         {
                             Name = x.FirstName + " " + x.LastName,
                             x.DoctorID
                         };

            doctorDropDownList.DataTextField = "Name";
            doctorDropDownList.DataValueField = "DoctorID";
            doctorDropDownList.DataSource = result;
            doctorDropDownList.DataBind();
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            startDateTextBox.Text = Calendar1.SelectedDate.ToShortDateString();
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            dbcontext.Appointments.Load();
            if (!IsPostBack)
            {
                //get doctor
                SQL.HealthclinicEntities dbcon = new SQL.HealthclinicEntities();
                int docIDT = Convert.ToInt32(doctorDropDownList.SelectedValue);
                SQL.Doctor doc = (from x in dbcon.Doctors
                                  where x.DoctorID == docIDT
                                  select x).First();

                //Appointments are all a half hour long. Thanks Oksana!
                TimeSpan aptLength = new TimeSpan(0, 0, 30);
                DateTime aptDate = DateTime.Parse(startDateTextBox.Text);
                //Get all of doctors appointments on the day of the appointment
                List<SQL.Appointment> aptList = (from x in dbcon.Appointments
                                                 where x.DoctorID == doc.DoctorID && x.AppointmentDate == aptDate
                                                 select x).ToList();
                TimeSpan aptTime = TimeSpan.Parse(appointmentsDropDownList.SelectedValue);
                
                //check to see if time is valid
                foreach (SQL.Appointment cur_Apt in aptList)
                {
                    if (cur_Apt.AppointmentTime >= aptTime && cur_Apt.AppointmentTime <= aptTime.Add(aptLength)) throw new Exception("Time is invalid!");
                }
            }
            
            
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            IEnumerable<SQL.Appointment> testAppointmentList =
                    (from testDate in dbcontext.Appointments.Local
                     where testDate.AppointmentDate.Equals(startDateTextBox.Text)
                     orderby testDate.AppointmentTime
                     select testDate).AsEnumerable();
            foreach(SQL.Appointment testDate in testAppointmentList)
            {
                if(appointmentsDropDownList.SelectedValue.Equals(testDate.AppointmentTime))
                {
                    appointmentsDropDownList.SelectedValue.Remove(appointmentsDropDownList.SelectedIndex);
                }
            }


            /*
            var userID = User.Identity.GetUserId();

            dbcontext.Patients.Load();
            //SQL.Patient myPatient = new SQL.Patient();

            SQL.Patient myPatient = (from y in dbcontext.Patients.Local
                                            where y.Email.Equals(userID)
                                            select y).First();
            SQL.Patient np = myPatient;
            */
            dbcontext.Appointments.Load();
            SQL.Patient patient = (from x in dbcontext.Patients
                                   where x.Email.Equals(User.Identity.Name)
                                   select x).First();

            SQL.Appointment myAppointment = new SQL.Appointment();
            myAppointment.AppointmentDate = Convert.ToDateTime(startDateTextBox.Text);
            myAppointment.AppointmentTime = TimeSpan.Parse(appointmentsDropDownList.SelectedValue);
            myAppointment.PatientID = patient.PatientID;
            myAppointment.DoctorID = Convert.ToInt32(doctorDropDownList.SelectedValue);
            myAppointment.HospitalID = Convert.ToInt32(locationDropDownList.SelectedValue);
            myAppointment.AppointmentLocation = Convert.ToInt32(locationDropDownList.SelectedValue);
            myAppointment.DepartmentID = Convert.ToInt32(departmentDropDownList.SelectedValue);
            myAppointment.Reason = reasonTextBox.Text;

            dbcontext.Appointments.Add(myAppointment);
            dbcontext.SaveChanges();
        
        }
    }
}