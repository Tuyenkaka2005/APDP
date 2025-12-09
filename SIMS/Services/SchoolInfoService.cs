using System;

namespace SIMS.Services
{
    public interface ISchoolInfoService
    {
        string GetSchoolName();
        string GetSchoolAddress();
        string GetContactEmail();
    }

    public class SchoolInfoService : ISchoolInfoService
    {
        private readonly string _schoolName;
        private readonly string _schoolAddress;
        private readonly string _contactEmail;

        // Singleton: Private constructor or managed by DI container as Singleton
        public SchoolInfoService()
        {
            _schoolName = "BTEC FPT British College Hanoi (BTEC FPT Hanoi)";
            _schoolAddress = "FPT BTEC Building, 13 Phan Tay Nhac Street, Xuan Phuong Ward, Hanoi City";
            _contactEmail = "btecfpt@fe.edu.vn";
        }

        public string GetSchoolName() => _schoolName;
        public string GetSchoolAddress() => _schoolAddress;
        public string GetContactEmail() => _contactEmail;
    }
}
