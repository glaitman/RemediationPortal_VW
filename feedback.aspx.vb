Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports System.Web.Configuration

Partial Class _feedback
    Inherits BetterPage

    Friend Status As Byte = 0
    Friend MAIL_SERVER_ADDR As String = Convert.ToString(WebConfigurationManager.AppSettings("MailServer"))
    Friend FEEDBACK_EMAIL As String = Convert.ToString(WebConfigurationManager.AppSettings("FeedbackEmail"))
    Friend Const FEEDBACK_SUBJECT As String = "Portal Feedback"
    Friend Const AUTOREPLY_EMAIL As String = "Automated Response <no-reply@lfr.com>"
    Friend Const AUTOREPLY_SUBJECT As String = "Portal Feedback Autoresponse"
    Friend Const REDIRECT_PATH As String = "thanks.html"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                Status = 1
                lblError.Text = ""
                URL.ToolTip = "If applicable, please provide the URL."
                Message.ToolTip = "Enter the question here."

                'get e-mail address and pre-populate
                Email.Text = SessionHandler.Read("UserEmail")
            Catch
                Throw
            End Try
        Else
            Status = 2
            'server side validation
            Dim rx As New Regex("\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
            If Not rx.Match(Email.Text).Success Then
                lblError.Text = "Please enter a valid e-mail address."
                lblError.Focus()
                Exit Sub
            End If

            If Message.Text.Trim = "" Then
                lblError.Text = "Please provide a detailed question."
                Message.Focus()
                Exit Sub
            End If

            'initialize SMTP client
            Dim c As SmtpClient = Nothing
            Try
                c = New SmtpClient(MAIL_SERVER_ADDR)
                c.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis
            Catch ex As Exception
                Throw New Exception("Could not initialize SMTP client.", ex)
                Exit Sub
            End Try
            If c Is Nothing Then
                Throw New Exception("Could not initialize SMTP client.", Nothing)
                Exit Sub
            End If

            lblError.Text = ""
            Status = 3
            'create feedback message
            Const EOL As String = Chr(13) & Chr(10)
            Dim BodyText As String
            BodyText = "URL: " & URL.Text.Trim & EOL
            BodyText &= EOL
            BodyText &= Message.Text.Trim & EOL

            'create feedback mail
            Dim Msg As MailMessage = Nothing
            Try
                Msg = New MailMessage(Email.Text, FEEDBACK_EMAIL)
                Msg.Subject = FEEDBACK_SUBJECT
                Msg.Body = BodyText
                Msg.SubjectEncoding = Encoding.ASCII
            Catch ex As Exception
                Throw New Exception("Could not create feedback mail.", ex)
                Msg.Dispose()
                Msg = Nothing
                Exit Sub
            End Try

            'send feedback mail
            Try
                c.Send(Msg)
            Catch ex As Exception
                Throw New Exception("Could not send feedback mail", ex)
                Msg.Dispose()
                Msg = Nothing
                Exit Sub
            End Try

            'cleanup feedback mail
            Msg.Dispose()
            Msg = Nothing

            'create confirmation message
            BodyText = "Thank you. This feedback has been forwarded to our study team. " & EOL
            BodyText = "Please keep a copy of this message for reference. Content follows:" & EOL
            BodyText &= EOL
            BodyText &= "From: " & Email.Text.Trim & EOL
            BodyText &= "URL: " & URL.Text.Trim & EOL
            BodyText &= EOL
            BodyText &= Message.Text.Trim & EOL
            BodyText &= EOL
            'BodyText &= "Regards," & EOL
            'BodyText &= "LFR, Inc." & EOL
            'BodyText &= EOL
            BodyText &= "PLEASE DO NOT REPLY TO THIS MESSAGE! "
            BodyText &= "This is an automated response from the server and does not "
            BodyText &= "necessarily mean your feedback has been received yet."

            'create confirmation mail
            Try
                Msg = New MailMessage(AUTOREPLY_EMAIL, Email.Text)
                Msg.Subject = AUTOREPLY_SUBJECT
                Msg.Body = BodyText
                Msg.SubjectEncoding = Encoding.ASCII
            Catch ex As Exception
                Throw New Exception("Could not create confirmation", ex)
                Msg.Dispose()
                Msg = Nothing
                Exit Sub
            End Try

            'send confirmation mail
            Try
                c.Send(Msg)
            Catch ex As Exception
                Throw New Exception("Could not send confirmation", ex)
                Throw New HttpException
                Msg.Dispose()
                Msg = Nothing
                Exit Sub
            End Try

            'cleanup confirmation mail
            Msg.Dispose()
            Msg = Nothing
            c = Nothing

            Status = 4
            Response.Redirect(REDIRECT_PATH, False)
        End If
    End Sub
End Class
