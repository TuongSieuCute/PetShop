using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Helper;
using Pet_Shop2.Models;
using System.Net.Mail;

namespace Pet_Shop2.Controllers
{
    public class SendMailController : Controller
    {

        private readonly PetShopContext db;
        public SendMailController(PetShopContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult SendMail(int id = -1, string toEmail = "")
        {


            if (!string.IsNullOrEmpty(toEmail) && id != -1)
            {
                string listproduct = "";
                Order? ord = db.Orders.SingleOrDefault(x => x.Id == id);
                List<OrderDetail> order = db.OrderDetails
                    .Where(x => x.OrderId == id).ToList();
                if (order != null)
                {
                    int index = 1;
                    foreach (var item in order)
                    {
                        Product? prd = db.Products.SingleOrDefault(x => x.Id == item.ProductId);
                        listproduct += $@"
    <tr>
        <td width='10%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px;'>{index}</td>
        <td width='35%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px;'>{prd?.ProductName}</td>
        <td width='25%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px;'>{prd?.Price}</td>
        <td width='10%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px;'>{item.Quantity}</td>
        <td width='20%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px;'>{Utilities.ToVND((double)item.Total)}</td>
    </tr>";
                        index++;
                    }
                }
                string total = $@"<tr>
                                    <td width='60%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px; border-top: 3px solid #eeeeee; border-bottom: 3px solid #eeeeee;'>
                                        Tổng 
                                    </td>
                                    <td width='10%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px; border-top: 3px solid #eeeeee; border-bottom: 3px solid #eeeeee;'>
                                        
                                    </td>
                                    <td width='10%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px; border-top: 3px solid #eeeeee; border-bottom: 3px solid #eeeeee;'>
                                         {order?.Sum(x => x.Quantity)}
                                    </td>
                                    <td width='25%' align='left' style='font-family:system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px; border-top: 3px solid #eeeeee; border-bottom: 3px solid #eeeeee;'>
                                        {Utilities.ToVND((double)order.Sum(x => x.Total))}
                                    </td>
                                </tr>";
                int orderID = id;
                string? address = ord?.Address;
                string? shipdate = ord?.ShipDate.ToString();
                string style = @"body, table, td, a { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }
table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }
img { -ms-interpolation-mode: bicubic; }

img { border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }
table { border-collapse: collapse !important; }
body { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }


a[x-apple-data-detectors] {
    color: inherit !important;
    text-decoration: none !important;
    font-size: inherit !important;
    font-family: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
}

@media screen and (max-width: 480px) {
    .mobile-hide {
        display: none !important;
    }
    .mobile-center {
        text-align: center !important;
    }
}
div[style*='margin: 16px 0;'] { margin: 0 !important; }";
                string Mailbody = $@"<!DOCTYPE html>
<html>  
<head>
<title></title>
<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
<meta name='viewport' content='width=device-width, initial-scale=1'>
<meta http-equiv='X-UA-Compatible' content='IE=edge' />
<style type='text/css'>
{style}
</style>
<body style='margin: 0 !important; padding: 0 !important; background-color: #eeeeee;' bgcolor='#eeeeee'>

<table border='0' cellpadding='0' cellspacing='0' width='100%'>
    <tr>
        <td align='center' style='background-color: #eeeeee;' bgcolor='#eeeeee'>
        
        <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;'>
            <tr>
                <td align='center' valign='top' style='font-size:0; padding: 35px;' bgcolor='#259cd8'>
               
                <div style='display:inline-block; max-width:50%; min-width:100px; vertical-align:top; width:100%;'>
                    <table align='left' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:300px;'>
                        <tr>
                            <td align='left' valign='top' style='font-family:   system-ui; font-size: 36px; font-weight: 800; line-height: 48px;' class='mobile-center'>
                                <h1 style='font-size: 36px; font-weight: 800; margin: 0; color: #ffffff;'>PetShop</h1>
                            </td>
                        </tr>
                    </table>
                </div>
                
                <div style='display:inline-block; max-width:50%; min-width:100px; vertical-align:top; width:100%;' class='mobile-hide'>
                    <table align='left' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:300px;'>
                        <tr>
                            <td align='right' valign='top' style='font-family:   system-ui; font-size: 48px; font-weight: 400; line-height: 48px;'>
                                <table cellspacing='0' cellpadding='0' border='0' align='right'>
                                    <tr>
                                        <td style='font-family:   system-ui; font-size: 18px; font-weight: 400;'>
                                            <p style='font-size: 18px; font-weight: 400; margin: 0; color: #ffffff;'><a href='#' target='_blank' style='color: #ffffff; text-decoration: none;'>Shop &nbsp;</a></p>
                                        </td>
                                        <td style='font-family:   system-ui; font-size: 18px; font-weight: 400; line-height: 24px;'>
                                            <a href='#' target='_blank' style='color: #ffffff; text-decoration: none;'><img src='https://cdn-icons-png.flaticon.com/512/6915/6915463.png' width='27' height='23' style='display: block; border: 0px;'/></a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
              
                </td>
            </tr>
            <tr>
                <td align='center' style='padding: 35px 35px 20px 35px; background-color: #ffffff;' bgcolor='#ffffff'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;'>
                    <tr>
                        <td align='center' style='font-family: system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding-top: 25px;'>
                            <img src='https://img.icons8.com/carbon-copy/100/000000/checked-checkbox.png' width='125' height='120' style='display: block; border: 0px;' /><br>
                            <h2 style='font-size: 30px; font-weight: 800; line-height: 36px; color: #333333; margin: 0;'>
                                Cảm ơn bạn đã đặt hàng!
                            </h2>
                        </td>
                    </tr>
                    <tr>
                        <td align='left' style='font-family:   system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding-top: 10px;'>
                            <p style='font-size: 16px; font-weight: 400; line-height: 24px; color: #777777;'>
                                Cảm ơn bạn đã đặt hàng từ của hàng chúng tôi. Hy vọng bạn sẽ có một trải nghiệm thật thú vị !
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td align='left' style='font-family:   system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding-top: 5px;'>
                            <p style='font-size: 16px;margin: 0px; font-weight: 400; line-height: 24px; color: #777777;'>
                                Mã đơn hàng : <b>{orderID}</b> 
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td align='left' style='padding-top: 20px;'>
                            <table cellspacing='0' cellpadding='0' border='0' width='100%'>
                                <tr>
                                    <td width='10%' align='left' bgcolor='#eeeeee' style='font-family:   system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px;'>
                                        STT
                                    </td>
                                    <td width='35%' align='left' bgcolor='#eeeeee' style='font-family:   system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px;'>
                                        Tên SP
                                    </td>
                                    <td width='25%' align='left' bgcolor='#eeeeee' style='font-family:   system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px;'>
                                        Giá SP
                                    </td>
                                    <td width='10%' align='left' bgcolor='#eeeeee' style='font-family:   system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px;'>
                                        SL
                                    </td>
                                    <td width='20%' align='left' bgcolor='#eeeeee' style='font-family:   system-ui; font-size: 16px; font-weight: 800; line-height: 24px; padding: 10px;'>
                                        Giá
                                    </td>
                                </tr>
                                {listproduct}
                            
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align='left' style='padding-top: 20px;'>
                            <table cellspacing='0' cellpadding='0' border='0' width='100%'>
                                {total}  
                            </table>
                        </td>
                    </tr>
                </table>
                
                </td>
            </tr>
             <tr>
                <td align='center' height='100%' valign='top' width='100%' style='padding: 0 35px 35px 35px; background-color: #ffffff;' bgcolor='#ffffff'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:660px;'>
                    <tr>
                        <td align='center' valign='top' style='font-size:0;'>
                            <div style='display:inline-block; max-width:50%; min-width:240px; vertical-align:top; width:100%;'>

                                <table align='left' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:300px;'>
                                    <tr>
                                        <td align='left' valign='top' style='font-family:   system-ui; font-size: 16px; font-weight: 400; line-height: 24px;'>
                                            <p style='font-weight: 800;'>Đ/C Giao hàng</p>
                                            <p style='width:140px;'>{address}</p>

                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div style='display:inline-block; max-width:50%; min-width:240px; vertical-align:top; width:100%;'>
                                <table align='left' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:300px;'>
                                    <tr>
                                        <td align='left' valign='top' style='font-family:   system-ui; font-size: 16px; font-weight: 400; line-height: 24px;'>
                                            <p style='font-weight: 800;'>Dự kiến ngày giao</p>
                                            <p>{shipdate}</p>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
                </td>
            </tr>
            <tr>
                <td align='center' style=' padding: 35px; background-color: #259cd8;' bgcolor='#259cd8'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;'>
                    <tr>
                        <td align='center' style='font-family:   system-ui; font-size: 16px; font-weight: 400; line-height: 24px; padding-top: 25px;'>
                            <h2 style='font-size: 24px; font-weight: 800; line-height: 30px; color: #ffffff; margin: 0;'>
                                Bạn sẽ được FreeShip ở lần đặt tiếp theo !
                            </h2>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='padding: 25px 0 15px 0;'>
                            <table border='0' cellspacing='0' cellpadding='0'>
                                <tr>
                                    <td align='center' style='border-radius: 5px;' bgcolor='#259cd8'>
                                      <a href='#' target='_blank' style='font-size: 18px; font-family:   system-ui; color: #ffffff; text-decoration: none; border-radius: 5px; background-color: #F44336; padding: 15px 30px; border: 1px solid #F44336; display: block;'>Quay lại</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                </td>
            </tr>
            <tr>
                <td align='center' style='padding: 35px; background-color: #ffffff;' bgcolor='#ffffff'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;'>
                    <tr>
                        <td align='center'>
                            <img src='logo-footer.png' width='37' height='37' style='display: block; border: 0px;'/>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='font-family:   system-ui; font-size: 14px; font-weight: 400; line-height: 24px; padding: 5px 0 10px 0;'>
                            <a href='https://maps.app.goo.gl/3Q81MUJ1JzMniV2HA' style='font-size: 14px; font-weight: 800; line-height: 18px; color: #333333;'>
                                96/3 Nguyễn Huệ,Vạn giã,<br>
                                Vạn Ninh,Khánh Hòa
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td align='left' style='font-family:   system-ui; font-size: 14px; font-weight: 400; line-height: 24px;'>
                            <p style='font-size: 14px; font-weight: 400; line-height: 20px; color: #777777;'>
                                Nếu bạn có thắc mắc. Vui lòng liên hệ qua <a href='#' target='_blank' style='color: #777777;'>Email: trankhoa192837@gmail.com hoặc SĐT:0966160922</a>.
                            </p>
                        </td>
                    </tr>
                </table>
                </td>
            </tr>
        </table>
        </td>
    </tr>
</table>
    
</body>
</html>
";






                string subject = "THƯ ĐẶT HÀNG THÀNH CÔNG !";
                string mailTitle = "[Cửa hàng PetShop !]";
                string fromEmail = "trankhoa192837@gmail.com";
                string fromEmailPassword = "nnpbsdgxqcrdxcyj";

                //Email Content
                MailMessage mailMessage = new MailMessage(new MailAddress(fromEmail, mailTitle), new MailAddress(toEmail));
                mailMessage.Subject = subject;
                mailMessage.Body = Mailbody;
                mailMessage.IsBodyHtml = true;

                //Server Details
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                //Credentials
                System.Net.NetworkCredential credential = new System.Net.NetworkCredential();
                credential.UserName = fromEmail;
                credential.Password = fromEmailPassword;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credential;

                smtp.Send(mailMessage);
            }
            

            return Json(new { success = true });
        }
    }
}
