using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace Assignment3.Messaging
{
    public partial class Messaging : System.Web.UI.Page
    {
        SQL.HealthclinicEntities dbcontext = new SQL.HealthclinicEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
           
                dbcontext.Doctors.Load();
            SQL.Patient me = (from x in dbcontext.Patients
                              where x.Email.Equals(User.Identity.Name)
                              select x).First();

            List<SQL.Message> msgList = (from x in dbcontext.Messages
                                         where x.TO.Equals(User.Identity.Name)
                                         select x).ToList();

            GridView1.DataSource = msgList;
            GridView1.DataBind();

            if (!IsPostBack)
                {
                var result = from x in dbcontext.Doctors.Local
                             select new
                             {
                                 Name = x.FirstName + ", " + x.LastName,
                                 x.Email
                             };

                    DropDownList1.DataTextField = "Name";
                    DropDownList1.DataValueField = "Email";
                    DropDownList1.DataSource = result;
                    DropDownList1.DataBind();
                }


        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            using (SQL.HealthclinicEntities dbcon = new SQL.HealthclinicEntities())
            {
                //dbcon.Patients.Load();
                //SQL.Patient myPatient = dbcon.Patients.Local.First();

                dbcon.Messages.Load();
                SQL.Message myMsg = new SQL.Message();

                


                //Label1.Text = usrID;

                
                myMsg.MessageID = dbcon.Messages.Local.Count + 1;
                myMsg.FROM = User.Identity.Name;
                myMsg.TO = Convert.ToString(DropDownList1.SelectedItem.Value);
                myMsg.Message1 = TextBox1.Text;

                dbcon.Messages.Add(myMsg);
                dbcon.SaveChanges();
            }
            
            GridView1.DataBind();
        }
    }
}