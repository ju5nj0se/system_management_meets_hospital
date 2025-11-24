using Microsoft.Extensions.Options;
using MimeKit;
using SystemManagementMeets.Models;
using SystemManagementMeets.ViewModels; 


namespace SistemaGestionCitasHospital.Services;

public class EmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    // --- Método Genérico de Envío (SendEmailAsync) ---
    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder();
        if (isHtml)
        {
            builder.HtmlBody = body;
        }
        else
        {
            builder.TextBody = body;
        }

        email.Body = builder.ToMessageBody();

        using (var client = new MailKit.Net.Smtp.SmtpClient()) 
        {
            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);
                
                // En SendGrid, _smtpSettings.Username debe ser "apikey"
                // y _smtpSettings.Password debe ser tu Clave API.
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Manejo de errores (registrar o relanzar la excepción)
                throw new InvalidOperationException("Error al enviar el correo con SendGrid (MailKit).", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }

    // --- Método Específico de Confirmación de Cita ---
    public async Task SendMeetConfirmationAsync(string toEmail, ManageMeetVM meetData, string doctorName)
    {
        // 1. Formateo de datos
        string fechaCita = meetData.DateMeet.ToShortDateString();
        // Usar formato 12 horas con indicador AM/PM (tt)
        string horaInicio = meetData.StartTimeDate.ToString("hh\\:mm tt"); 
        
        // 2. Construcción del Asunto y Cuerpo
        
        string subject = $"Confirmación de Cita el dia {fechaCita} a las {horaInicio}";

        // Cuerpo HTML (sin cambios, ya estaba bien)
        string bodyHtml = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
                    .header {{ background-color: #007bff; color: white; padding: 10px 0; text-align: center; border-radius: 5px 5px 0 0; }}
                    .details {{ padding: 20px 0; }}
                    .detail-row {{ margin-bottom: 10px; }}
                    .label {{ font-weight: bold; color: #555; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Cita Médica Confirmada</h2>
                    </div>
                    <div class='details'>
                        <p>Estimado/a paciente,</p>
                        <p>Confirmamos su cita médica. Por favor, asegúrese de llegar 15 minutos antes de la hora programada.</p>
                        
                        <div class='detail-row'>
                            <span class='label'>Médico:</span> {doctorName}
                        </div>
                        <div class='detail-row'>
                            <span class='label'>Fecha:</span> <strong>{fechaCita}</strong>
                        </div>
                        <div class='detail-row'>
                            <span class='label'>Hora:</span> <strong>{horaInicio}</strong>
                        </div>
                        <div class='detail-row'>
                            <span class='label'>Duración estimada:</span> {meetData.StartTimeDate} - {meetData.EndTimeDate}
                        </div>
                        <p>¡Gracias por usar nuestros servicios!</p>
                    </div>
                </div>
            </body>
            </html>";

        // 3. Reutilizar el método de envío
        await SendEmailAsync(toEmail, subject, bodyHtml, isHtml: true);
    }
}