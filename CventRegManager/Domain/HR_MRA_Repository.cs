using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CventRegManager.Models;
using CventRegManager.MRACventAPI;
using CventRegManager.Interfaces;

namespace CventRegManager.Domain
{
    public class HR_MRA_Repository : IHR_MRA_Repository
    {
       
        public bool LogSimpleCall(string PostBodyData, string cventAuthHeader, string verb, bool Authorized)
        {
            if (Authorized) cventAuthHeader = "Valid Access Key";
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["HRSQL"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into  WebhooksMessageLog ( JsonMessageBody ,HttpHeader, RestVerb, AuthorizedCall  ) values (@JsonMessageBody, @HttpHeader, @RestVerb, @AuthorizedCall) ";
                cmd.Parameters.Add(new SqlParameter("@JsonMessageBody", PostBodyData.ToString()));
                cmd.Parameters.Add(new SqlParameter("@HttpHeader", cventAuthHeader));
                cmd.Parameters.Add(new SqlParameter("@RestVerb", verb));
                cmd.Parameters.Add(new SqlParameter("@AuthorizedCall", Authorized));
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return true;

            }
            catch (Exception ex)
            {
                SqlCommand fail_cmd = con.CreateCommand();
                fail_cmd.CommandType = CommandType.Text;
                fail_cmd.CommandText = "insert into  WebhooksMessageLog ( JsonMessageBody   ) values (@JsonMessageBody) ";
                fail_cmd.Parameters.Add(new SqlParameter("@JsonMessageBody", ex.Message.ToString()));

                con.Open();
                fail_cmd.ExecuteNonQuery();
                con.Close();

                return false;
            }
            finally
            {
                if (con != null) { con.Close(); }

            }

        }



        public string  RegProcessedStatus(string inviteeStub)
        {

            string processedGUID;
            string status = "Missing";
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["HRSQL"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select ProtechReturnGUID from   WebhooksAPAPinvitee where CventInviteeGUID = @CventInviteeGUID ";
                cmd.Parameters.Add(new SqlParameter("@CventInviteeGUID", inviteeStub));
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["ProtechReturnGUID"] != null)
                    {
                        processedGUID = reader["ProtechReturnGUID"].ToString();
                        if (processedGUID.Length == 36)
                        {
                            status = "Processed";

                        }
                        else
                        {

                            status = "Created";
                        }
                    }
                    else
                    {
                        status = "Created";
                    }
                }

                con.Close();
            }
            catch (Exception ex)
            {
              
            }
            finally
            {
                if (con != null) { con.Close(); }

            }
            return status;
        }
        public bool UpdateRegLogWithErrorMessage(CventAttendee att, string ErrorMessage)
        {
            bool UpdatedLogSuccessfully = false;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["HRSQL"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Update  WebhooksAPAPinvitee set ProtechReturnMessage = @ProtechReturnMessage where CventInviteeGUID = @CventInviteeGUID ";
                cmd.Parameters.Add(new SqlParameter("@ProtechReturnMessage", ErrorMessage));
                cmd.Parameters.Add(new SqlParameter("@CventInviteeGUID", att.cventInvitteeId));
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                UpdatedLogSuccessfully = true;

            }
            catch (Exception ex)
            {

                //if log previous log attempt fails, then just log the basics
                SqlCommand fail_cmd = con.CreateCommand();
                fail_cmd.CommandType = CommandType.Text;
                fail_cmd.CommandText = "insert into  WebhookLog ( JsonMessageBody   ) values (@JsonMessageBody) ";
                fail_cmd.Parameters.Add(new SqlParameter("@JsonMessageBody", ex.Message.ToString()));
                if (con != null) { con.Close(); }
                con.Open();
                fail_cmd.ExecuteNonQuery();
                con.Close();

                UpdatedLogSuccessfully = false;
            }
            finally
            {
                if (con != null) { con.Close(); }

            }

            return UpdatedLogSuccessfully;
        
        }

        public bool UpdateRegLogAsProcessed(CventAttendee att)
        {
            bool UpdatedLogSuccessfully = false;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["HRSQL"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Update  WebhooksAPAPinvitee set ProtechReturnMessage='', ProtechReturnGUID = @ProtechReturnGUID where CventInviteeGUID = @CventInviteeGUID ";
                cmd.Parameters.Add(new SqlParameter("@ProtechReturnGUID", att.ProcessedGUID));
                cmd.Parameters.Add(new SqlParameter("@CventInviteeGUID", att.cventInvitteeId));
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                UpdatedLogSuccessfully = true;

            }
            catch (Exception ex)
            {

                //if log previous log attempt fails, then just log the basics
                SqlCommand fail_cmd = con.CreateCommand();
                fail_cmd.CommandType = CommandType.Text;
                fail_cmd.CommandText = "insert into  WebhookLog ( JsonMessageBody   ) values (@JsonMessageBody) ";
                fail_cmd.Parameters.Add(new SqlParameter("@JsonMessageBody", ex.Message.ToString()));
                if (con != null) { con.Close(); }
                con.Open();
                fail_cmd.ExecuteNonQuery();
                con.Close();

                UpdatedLogSuccessfully = false;
            }
            finally
            {
                if (con != null) { con.Close(); }

            }

            return UpdatedLogSuccessfully;
        }

        public bool CreateRegLog(CventAttendee att, string jsonMsgBody,  string eventStub)
        {
            //Attempt to log all information about registered attendee
            bool LoggedCallSuccessfully = false;
            DateTime dt;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["HRSQL"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                dt = Convert.ToDateTime(att.regDate);
            }
            catch 
            {
                dt = DateTime.Now;
            }

            bool SpouseReg = (att.guestName.Length > 0) ? true : false;
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into  WebhooksAPAPinvitee ( CreateDate, InviteeObjectSerialized , ConfirmationNumber,  PersonMembershipId, OrgMembershipId, RegType,  firstname, lastname, email, eventId, ExcludeEmail,FirstTime, Volunteer, HeardAboutEvent, CventInviteeGUID, RegDate, NumTicketsPurchased, paymentMethod, attendLuncheon, SponsorAwardTable, SpouseReg   ) values (@CreateDate, @InviteeObjectSerialized,  @ConfirmationNumber,  @PersonMembershipId, @OrgMembershipId, @RegType,   @firstname, @lastname, @email, @eventId, @ExcludeEmail,@FirstTime, @Volunteer, @HeardAboutEvent, @CventInviteeGUID, @RegDate, @NumTicketsPurchased, @paymentMethod, @attendLuncheon, @SponsorAwardTable, @SpouseReg) ";
                cmd.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@InviteeObjectSerialized", jsonMsgBody));
                cmd.Parameters.Add(new SqlParameter("@ConfirmationNumber", att.confirmationNumber));
                cmd.Parameters.Add(new SqlParameter("@PersonMembershipId", att.personMembershipId));
                cmd.Parameters.Add(new SqlParameter("@OrgMembershipId", att.orgMembershipId));
                cmd.Parameters.Add(new SqlParameter("@RegType", att.contactType));
                cmd.Parameters.Add(new SqlParameter("@firstname", att.firstName));
                cmd.Parameters.Add(new SqlParameter("@lastname", att.lastName));
                cmd.Parameters.Add(new SqlParameter("@email", att.email));
                cmd.Parameters.Add(new SqlParameter("@eventId", eventStub));
                cmd.Parameters.Add(new SqlParameter("@ExcludeEmail", att.OptOut));
                cmd.Parameters.Add(new SqlParameter("@FirstTime", att.FirstTime));
                cmd.Parameters.Add(new SqlParameter("@Volunteer", att.Volunteer));
                cmd.Parameters.Add(new SqlParameter("@HeardAboutEvent", att.HeardAboutValue));
                cmd.Parameters.Add(new SqlParameter("@CventInviteeGUID", att.cventInvitteeId));
                cmd.Parameters.Add(new SqlParameter("@RegDate", dt));
                cmd.Parameters.Add(new SqlParameter("@NumTicketsPurchased", att.LuncheonTicketsPurchased));
                cmd.Parameters.Add(new SqlParameter("@paymentMethod", att.paymentMethod));
                cmd.Parameters.Add(new SqlParameter("@attendLuncheon", att.AttendLuncheon));
                cmd.Parameters.Add(new SqlParameter("@SponsorAwardTable", att.SponsorLunchTable));
                cmd.Parameters.Add(new SqlParameter("@SpouseReg", SpouseReg));


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                LoggedCallSuccessfully = true;

            }
            catch (Exception ex)
            {

                //if log previous log attempt fails, then just log the basics
                SqlCommand fail_cmd = con.CreateCommand();
                fail_cmd.CommandType = CommandType.Text;
                fail_cmd.CommandText = "insert into  WebhooksMessageLog ( JsonMessageBody   ) values (@JsonMessageBody) ";
                fail_cmd.Parameters.Add(new SqlParameter("@JsonMessageBody", ex.Message.ToString()));
                if (con != null) { con.Close(); }
                con.Open();
                fail_cmd.ExecuteNonQuery();
                con.Close();

                LoggedCallSuccessfully = false;
            }
            finally
            {
                if (con != null) { con.Close(); }

            }
            return LoggedCallSuccessfully;
        }

        

    }
}