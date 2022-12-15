import smtplib, os
from email.mime.text import MIMEText
from dotenv import load_dotenv

def send_email(lh_name, email_list, contents):
    # Load the email/password
    load_dotenv()
    email_address = os.environ.get('email')
    email_password = os.environ.get('email_password')
    
    # Build the message
    contents += ("\n\n\nYou have received this email because you are subscribed to "
                f"the {lh_name.title()} Lighthouse and a notification threshold has been met.")
    msg = MIMEText(contents)
    msg['From'] = email_address
    msg['To'] = ', '.join(email_list)
    msg['Subject'] = f'InfoRadar Notification - {lh_name.title()} Lighthouse'
    
    # Create session
    session = smtplib.SMTP('smtp.gmail.com', 587)
    session.ehlo()
    session.starttls()
    # Login, send mail and quit
    session.login(email_address, email_password)
    session.sendmail(email_address, email_list, msg.as_string())
    session.quit()
    