using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class TableProcess : IDisposable
{
    public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\PROJECT DOOR\GYMMEMBER\GYMMEMBER\DATATABLES\GYMMEMBERDATABASES.MDF;Integrated Security=True";//@ sau chuoi string co dau /
    bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                //dispose managed resources
            }
        }
        //dispose unmanaged resources
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int CheckInAndOut(string varCardUID) // Mau Hoc ve SQL rat tot
    {
        using (SqlConnection ServerConnect = new SqlConnection(connectionString)) // using de giai phong tai nguyen khi dung xong
        {
            int OUT;
            ServerConnect.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ServerConnect; // ket noi
            cmd.CommandText = "Select MemberID,IsCancel,FName,LName from Tb1_PerInfor Where CardUID = @UID";
            // cmd.CommandText = "getCheckinoutInfor"; // is StoredProcedures
            // cmd.CommandType = CommandType.StoredProcedure; // Phai co kieu Truy van
            cmd.Parameters.AddWithValue("@UID", varCardUID);

            SqlDataReader sqlreader = cmd.ExecuteReader(); // Thi hanh: getCheckinoutInfor
            if (sqlreader.HasRows) // false la ko co du lieu, true la co du lieu // xem them khach khoi tai 
            {
                if (sqlreader.Read())
                {
                    var MID = sqlreader.GetInt32(0); // var MID = sqlreader.["MemberID"];   
                    var Isca = sqlreader.GetBoolean(1);//var Isca = sqlreader["IsCancel"];
                    if (!Isca) //XEM LAI KIEU BIEN BOOL???????????
                    {
                        OUT = 1;
                        //Turn Off Temporary
                        //StoreCheckinout(MID);
                    }
                    else
                    {
                        OUT = 0;
                    }
                }
                else
                {
                    OUT = 0;
                }
            }
            else
            {
                OUT = 0;
            }

            cmd.Dispose();
            sqlreader.Close();
            ServerConnect.Close();//Step 4: Close connection of SQL
            return OUT;
        }
    }

    public void StoreCheckinout(int vMemberID)
    // At president, SQL Database  still connected. Other Table will be connectd        
    {
        string QueryString = "";
        using (SqlConnection ServerConnect = new SqlConnection(connectionString)) // using de giai phong tai nguyen khi dung xong
        {
            ServerConnect.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ServerConnect; // ket noi       
            cmd.CommandText = "select * from Tb4_WorkOut where MemberID=@MemberID and DateMonth=@Today"; // is StoredProcedures           
            var id = new SqlParameter("@Today", DateTime.Now.Date.ToString("yyyy-MM-dd"));// khoi tao tham so ban dau            
            var id2 = new SqlParameter("@MemberID", vMemberID);// khoi tao tham so ban dau
            cmd.Parameters.Add(id);
            cmd.Parameters.Add(id2);

            SqlDataReader sqlreader = cmd.ExecuteReader(); // Thi hanh: getCheckinoutInfor
            if (sqlreader.HasRows) // false la ko co du lieu, true la co du lieu
            {
                if (sqlreader.Read())  // Checkin1?
                {
                    var co1 = sqlreader["Checkout1"];//var co1 = sqlreader.GetDateTime(1);
                    var ci2 = sqlreader["Checkin2"];
                    var co2 = sqlreader["Checkout2"];

                    if (string.IsNullOrEmpty(co1.ToString())) // Checkout1?
                    {
                        QueryString = "UPDATE Tb4_WorkOut SET Checkout1=@CheckContent where MemberID=@MemberID and DateMonth=@Today";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ci2.ToString()))  // Checkin 2?
                        {
                            QueryString = "UPDATE Tb4_WorkOut SET Checkin2=@CheckContent where MemberID=@MemberID and DateMonth=@Today";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(co2.ToString())) // Checkout2?
                            {
                                QueryString = "UPDATE Tb4_WorkOut SET Checkout2=@CheckContent where MemberID=@MemberID and DateMonth=@Today";
                            }
                            else
                            {
                                goto out1;
                            }
                        }
                    }
                    UpdateData(vMemberID, QueryString);//string QueryString; thi sai.. ma : string QueryString="" thi dung??                    }
                }
                else
                {
                    InsertData(vMemberID);
                }
            }
            else
            {
                InsertData(vMemberID);
            }
        out1:
            cmd.Dispose();
            ServerConnect.Close();
        }
    }
    public void InsertData(int vMemberID)
    {
        using (SqlConnection ServerConnect = new SqlConnection(connectionString)) // using de giai phong tai nguyen khi dung xong
        {
            ServerConnect.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ServerConnect; // ket noi       
            cmd.CommandText = "Insert into [Tb4_WorkOut] (Tb4_WorkOut.MemberID,DateMonth,Checkin1) values (@ID,@dm,@ci1)";

            var id1 = cmd.Parameters.AddWithValue("@ID", vMemberID);
            var id2 = cmd.Parameters.AddWithValue("@dm", DateTime.Now.Date);
            var ci1 = cmd.Parameters.AddWithValue("@ci1", DateTime.Now);
            var kq = cmd.ExecuteNonQuery(); // Thi hanh: getCheckinoutInfor
            cmd.Dispose();
            ServerConnect.Close();
        }
    }
    public void UpdateData(int vMemberID, string vQueryString)
    {
        using (SqlConnection ServerConnect = new SqlConnection(connectionString)) // using de giai phong tai nguyen khi dung xong
        {
            ServerConnect.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ServerConnect; // ket noi    
            cmd.CommandText = vQueryString;

            var id1 = cmd.Parameters.AddWithValue("@MemberID", vMemberID);
            var id2 = cmd.Parameters.AddWithValue("@Today", DateTime.Now.Date);
            var id3 = cmd.Parameters.AddWithValue("@CheckContent", DateTime.Now);
            var kq = cmd.ExecuteNonQuery(); // Thi hanh: getCheckinoutInfor
            cmd.Dispose();
            ServerConnect.Close();
        }
    }
    public void UpdateStateOfPayment(int vMemberID) // Call for it before Saveing Data payment
    {
        using (SqlConnection ServerConnect = new SqlConnection(connectionString)) // using de giai phong tai nguyen khi dung xong
        {
            bool @IsNextPaid = true;
            ServerConnect.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ServerConnect; // ket noi     
            cmd.CommandText = "UPDATE Tb2_Payment SET IsNextPaid=@IsNextPaid where MemberID=@MemberID and IsNextPaid!=@IsNextPaid";
            var id1 = cmd.Parameters.AddWithValue("@MemberID", vMemberID);
            var id2 = cmd.Parameters.AddWithValue("@IsNextPaid", @IsNextPaid);
            var kq = cmd.ExecuteNonQuery(); // Thi hanh: getCheckinoutInfor      
            ServerConnect.Close();
        }
    }
}