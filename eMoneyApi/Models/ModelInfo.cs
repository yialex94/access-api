//using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eMoneyApi.Models
{
    public class ClientInformation
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Clients_Request clients { get; set; }
    }
    public class Clients_Request
    {
        public int totalClientCount { get; set; }
        public Clientlist[] clientList { get; set; }
    }

    public class Clientlist
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string homePhone { get; set; }
        public string cellPhone { get; set; }
        public string url { get; set; }
        public string ownerId { get; set; }
        public string designation { get; set; }
    }

    public class Plan_Request
    {
        public PlanList[] resources { get; set; }
    }

    public class PlanList
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class DetailedClient
    {
        public string id { get; set; }
        public Client client { get; set; }
        public object spouse { get; set; }
        public object address { get; set; }
        public string homePhone { get; set; }
        public object fax { get; set; }
        public string maritalStatus { get; set; }
        public string basePlanId { get; set; }
        public string ownerId { get; set; }
        public string designation { get; set; }
        public object[] externalIds { get; set; }
    }

    public class Client
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public object specialNeeds { get; set; }
        public bool inGoodHealth { get; set; }
        public object previousMarriages { get; set; }
        public object citizenship { get; set; }
        public string email { get; set; }
        public string cellPhone { get; set; }
        public Job job { get; set; }
    }

    public class Job
    {
        public string companyName { get; set; }
        public object address { get; set; }
        public string jobTitle { get; set; }
        public string businessEmail { get; set; }
        public object businessPhone { get; set; }
        public object businessFax { get; set; }
        public object yearsEmployed { get; set; }
        public string previousEmployerName { get; set; }
        public string previousJobTitle { get; set; }
        public object previousYearsEmployed { get; set; }
    }


    public class RootObject
    {
        public int totalUsers { get; set; }
        public UserRequest[] users { get; set; }
    }

    public class UserRequest
    {
        public string id { get; set; }
        public string office { get; set; }
        public string[] roles { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string displayName { get; set; }
        public string companyName { get; set; }
        public string businessEmail { get; set; }
    }


}