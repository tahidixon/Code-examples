using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;

namespace Assignment3.Patient_History
{
    public partial class History : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SQL.HealthclinicEntities dbcon = new SQL.HealthclinicEntities();

            dbcon.Appointments.Load();
            try
            {
                SQL.Patient patient = (from x in dbcon.Patients
                                       where x.Email.Equals(User.Identity.Name)
                                       select x).First();

                List<SQL.Appointment> apts = (from x in dbcon.Appointments
                                              where x.PatientID == patient.PatientID
                                              select x).ToList();
                
                aptGridView.DataSource = apts;
                //aptGridView.DataKeyNames = "AppointmentID";
                aptGridView.DataBind();
            }
            catch(InvalidOperationException)
            {
                throw;
            }
        }

        protected void aptGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
                SQL.HealthclinicEntities dbcon = new SQL.HealthclinicEntities();

            dbcon.Appointments.Load();

                SQL.Patient patient = (from x in dbcon.Patients
                                       where x.Email.Equals(User.Identity.Name)
                                       select x).First();

                int something = Convert.ToInt32(aptGridView.SelectedDataKey[0]);

                SQL.Appointment apt = (from x in dbcon.Appointments
                                       where x.AppointmentID == something
                                       select x).First();



                SQL.Doctor doc = (from x in dbcon.Doctors
                                  where x.DoctorID == apt.DoctorID
                                  select x).First();

                var testShite = (from a in dbcon.Appointments
                                 select new
                                 {
                                     aptID = a.AppointmentID,
                                     aptPatient = a.PatientID,
                                     aptDoctor = a.DoctorID,
                                     aptDate = a.AppointmentDate,
                                     aptTime = a.AppointmentTime,
                                     aptLoc = a.AppointmentLocation,
                                     aptDept = a.DepartmentID,
                                     aptHospital = a.HospitalID,
                                     aptReason = a.Reason
                                 }).ToList();
                string[] potentialKeys = { "AppointmentID" };


                aptGridView.DataSource = testShite;
                aptGridView.DataKeyNames = potentialKeys;

                lblPName.Text = "Patient name: " + patient.LastName + "," + patient.FirstName;
                lblDName.Text = "Doctor name: " + doc.LastName + "," + doc.FirstName;
                lblLoc.Text = "Appointment location: " + apt.AppointmentLocation;
                lblDate.Text = "Appointment date: " + apt.AppointmentDate + "@" + apt.AppointmentTime;
                lblReason.Text = "Reason: " + apt.Reason;

            
            //}
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            SQL.HealthclinicEntities dbcon = new SQL.HealthclinicEntities();
            if (dbcon != null)
            {
                dbcon.Dispose();
            }
            dbcon = new SQL.HealthclinicEntities();
                try
                {
                    dbcon.Appointments.Load();

                    int notSomething = Convert.ToInt32(aptGridView.SelectedDataKey[0]);

                    SQL.Appointment myAppointment = (from x in dbcon.Appointments
                                                     where x.AppointmentID == notSomething
                                                     select x).First();

                    dbcon.Appointments.Remove(myAppointment);
                    dbcon.SaveChanges();

                    var testShite = (from a in dbcon.Appointments
                                 select new
                                 {
                                     aptID = a.AppointmentID,
                                     aptPatient = a.PatientID,
                                     aptDoctor = a.DoctorID,
                                     aptDate = a.AppointmentDate,
                                     aptTime = a.AppointmentTime,
                                     aptLoc = a.AppointmentLocation,
                                     aptDept = a.DepartmentID,
                                     aptHospital = a.HospitalID,
                                     aptReason = a.Reason
                                 }).ToList();

                 //   aptGridView.DataSource = testShite;
                 //   aptGridView.DataBind();
                }
                catch
                {
                    throw;
                }
           
        }
    }
}