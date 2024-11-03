using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace CityNexus.People.Domain.VO;

public sealed record Email(string Value)
{
    public static implicit operator string(Email email) => email.Value;

    public static Email Create(string value)
    {
        try
        {
            var _mailAddress = new MailAddress(value);
            var email = new Email(value.Trim().ToLower());
            return email;
        }
        catch
        {
            throw new Exception("Invalid email address.");
        }
    }
};
