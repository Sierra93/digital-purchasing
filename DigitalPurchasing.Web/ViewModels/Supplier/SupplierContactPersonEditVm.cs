using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.ViewModels.Supplier
{
    public class SupplierContactPersonEditVm 
    {
        [HiddenInput]
        public Guid Id { get; set; }
        [HiddenInput]
        public Guid SupplierId { get; set; }
        [HiddenInput, DisplayName("Поставщик")]
        public string SupplierName { get; set; }
        
        [Required, DisplayName("Имя")]
        public string FirstName { get; set; }
        [Required, DisplayName("Фамилия")]
        public string LastName { get; set; }
        [Required, EmailAddress, DisplayName("E-mail")]
        public string Email { get; set; }

        [Required, DisplayName("Должность")]
        public string JobTitle { get; set; }

        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        [DisplayName("Моб. телефон")]
        public string MobilePhoneNumber { get; set; }

        [DisplayName("Использовать для отправки запросов и получения КП?")]
        public bool UseForRequests { get; set; }
    }
}
