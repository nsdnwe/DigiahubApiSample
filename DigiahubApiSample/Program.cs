using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

// For the full API specification see https://digiahub.com/swagger
//
// nuget Install-Package Newtonsoft.Json
// nuget Install-Package RestSharp

namespace DigiahubApiSample {
    class Program {

        // Use API_KEY "sandbox-mode" for testing.
        // Sandbox data is isolated per caller and removed automatically after 24 hours. Do not send any real or sensitive data in sandbox mode.
        // In production replace API_KEY with your company apikey from https://digiahub.com/ApiInterface/ApiKey
        // Do not post sample data to the production environment.

        const string API_KEY = "sandbox-mode";

        // Override with environment variable DIGIAHUB_API_URI when testing against another environment
        static readonly string BASE_URI = Environment.GetEnvironmentVariable("DIGIAHUB_API_URI") ?? "https://digiahub.com/api/";

        static void Main(string[] args) {

            if (API_KEY != "sandbox-mode") throw new Exception("Make sure you are not posting sample data to the production environment, then remove this check.");

            // ===============================================================================================
            // Lookup lists: use these values in the resource, language and education calls

            Console.WriteLine("GET /currencyList");
            Console.WriteLine(JsonConvert.SerializeObject(GetCurrencyList()));
            Console.WriteLine("GET /languageList");
            Console.WriteLine(JsonConvert.SerializeObject(GetLanguageList()));
            Console.WriteLine("GET /educationTypeList");
            Console.WriteLine(JsonConvert.SerializeObject(GetEducationTypeList()));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================
            // Post a new resource and use its ID in the next calls

            var newResource = new ApiResource() {
                Name = "Sample name",
                Email = "sample@email.com",
                Phone = "+358405001234",
                Location = "Helsinki",
                IsAvailable = true,
                AvailableFrom = "20.01.2027",
                PriceRequest = "95",
                Currency = "EUR",
                Summary = "Senior fullstack developer with 10 years of experience."
            };
            string resourceID = PostResource(newResource);

            Console.WriteLine("POST /resource");
            Console.WriteLine("New id: " + resourceID);
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var allResource = GetAllResources();

            Console.WriteLine("GET /resource");
            Console.WriteLine(JsonConvert.SerializeObject(allResource, Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var resource = GetResource(resourceID);

            Console.WriteLine("GET /resource/" + resourceID);
            Console.WriteLine(JsonConvert.SerializeObject(resource, Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var updatedResource = new ApiResource() {
                Name = "New name",
                Email = "example@email.com",
                IsAvailable = false,
                Phone = "+358405001234",
                Location = "Espoo",
                PriceRequest = "1000",
                Currency = "SEK",
                Summary = "Updated summary."
            };
            PutResource(resourceID, updatedResource);

            Console.WriteLine("PUT /resource");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var apiAvailability = new ApiAvailability() {
                AvailableFrom = "21.01.2027",
                IsAvailable = true
            };
            SetAvailability(resourceID, apiAvailability);

            Console.WriteLine("PUT /availability");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================
            // CV file. Allowed types: .pdf, .doc, .docx, .rtf, .txt, .jpg, .jpeg, .png. Max 10 MB.
            // The content must match the file type (for example a .pdf must start with %PDF).

            var apiResourceFile = new ApiResourceFile() {
                Base64Data = "VGhpcyBpcyBhIHNhbXBsZSBmb3IgQmFzZTY0IGNvbnZlcnRpb24u",
                FileName = "sample-cv.txt"
            };
            string newFileName = PostResourceFile(resourceID, apiResourceFile);

            // Sample code how to convert a file to a Base64 string
            // byte[] bytes = System.IO.File.ReadAllBytes("file");
            // string base64String = Convert.ToBase64String(bytes);

            Console.WriteLine("POST /resourceFile");
            Console.WriteLine("New file name: " + newFileName);
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            string resourceFileName = GetResourceFileName(resourceID);

            Console.WriteLine("GET /resourceFile");
            Console.WriteLine(resourceFileName);
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================
            // Profile image. Allowed types: .jpg, .jpeg, .png. Max 3 MB. This sample is a 1x1 pixel PNG.

            var profileImage = new ApiResourceFile() {
                Base64Data = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==",
                FileName = "sample-image.png"
            };
            string newImageName = PostProfileImage(resourceID, profileImage);

            Console.WriteLine("POST /profileImage");
            Console.WriteLine("New file name: " + newImageName);
            Console.WriteLine("GET /profileImage");
            Console.WriteLine(GetProfileImageFileName(resourceID));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var competenceList = GetCompetenceList();

            Console.WriteLine("GET /competenceList");
            Console.WriteLine(JsonConvert.SerializeObject(competenceList, Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================
            // Always post the full list, not just the changed item

            var newCompetences = new List<ApiCompetence>();
            newCompetences.Add(new ApiCompetence() { CompetenceName = "Java", WorkExperienceYears = 5.5 });
            newCompetences.Add(new ApiCompetence() { CompetenceName = "Paradox solving", WorkExperienceYears = 3 });
            PostCompetences(resourceID, newCompetences);

            Console.WriteLine("POST /competence");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var competences = GetResourceCompetences(resourceID);

            Console.WriteLine("GET /competence");
            Console.WriteLine(JsonConvert.SerializeObject(competences, Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================
            // Always post the full list, not just the changed item

            var newWorkHistory = new List<ApiWorkHistory>();
            newWorkHistory.Add(new ApiWorkHistory() { CompanyName = "NSD Consulting Oy", StartDate = "20.01.2010", EndDate = "31.12.2020", JobTitle = "Software developer", JobDescription = "Developing fullstack applications for web and mobile." });
            newWorkHistory.Add(new ApiWorkHistory() { CompanyName = "Digia Oyj", StartDate = "01.01.2021", EndDate = "", JobTitle = "System architect", JobDescription = "Designing high availability cloud applications." });
            PostWorkHistory(resourceID, newWorkHistory);

            Console.WriteLine("POST /workHistory");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var workHistory = GetResourceWorkHistory(resourceID);

            Console.WriteLine("GET /workHistory");
            Console.WriteLine(JsonConvert.SerializeObject(workHistory, Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================
            // Language skills. Language names must match GET /languageList. Levels: 1 = Basics, 2 = Intermediate, 3 = Fluent, 4 = Native.
            // Always post the full list.

            var newLanguages = new List<ApiLanguage>();
            newLanguages.Add(new ApiLanguage() { Language = "Finnish", Level = 4 });
            newLanguages.Add(new ApiLanguage() { Language = "English", Level = 3 });
            newLanguages.Add(new ApiLanguage() { Language = "Swedish", Level = 1 });
            PostLanguages(resourceID, newLanguages);

            Console.WriteLine("POST /language");
            Console.WriteLine("Done");
            Console.WriteLine("GET /language");
            Console.WriteLine(JsonConvert.SerializeObject(GetResourceLanguages(resourceID), Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================
            // Educations. Type must match GET /educationTypeList. EndDate in dd.MM.yyyy format, empty if ongoing.
            // Always post the full list.

            var newEducations = new List<ApiEducation>();
            newEducations.Add(new ApiEducation() { Type = "Education", Name = "M.Sc. Computer Science", Institute = "Aalto University", EndDate = "31.05.2015" });
            newEducations.Add(new ApiEducation() { Type = "Certificate", Name = "Azure Solutions Architect Expert", Institute = "Microsoft", EndDate = "" });
            PostEducations(resourceID, newEducations);

            Console.WriteLine("POST /education");
            Console.WriteLine("Done");
            Console.WriteLine("GET /education");
            Console.WriteLine(JsonConvert.SerializeObject(GetResourceEducations(resourceID), Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            DeleteProfileImage(resourceID);
            DeleteResourceFile(resourceID);

            Console.WriteLine("DELETE /profileImage, DELETE /resourceFile");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            DeleteResource(resourceID);

            Console.WriteLine("DELETE /resource");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");
        }

        // ===============================================================================================
        // Sample methods
        // ===============================================================================================

        static List<ApiResource> GetAllResources() {
            using (var client = createClient("resource")) {
                return client.Get<List<ApiResource>>(new RestRequest());
            }
        }

        static ApiResource GetResource(string id) {
            using (var client = createClient("resource/" + id)) {
                return client.Get<ApiResource>(new RestRequest());
            }
        }

        static string PostResource(ApiResource newResource) {
            using (var client = createClient("resource")) {
                var request = new RestRequest();
                request.AddJsonBody(newResource);
                return client.Post<string>(request);
            }
        }

        static void PutResource(string id, ApiResource newResource) {
            using (var client = createClient("resource/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(newResource);
                client.Put(request);
            }
        }

        static void DeleteResource(string id) {
            using (var client = createClient("resource/" + id)) {
                client.Delete(new RestRequest());
            }
        }

        static void SetAvailability(string id, ApiAvailability apiAvailability) {
            using (var client = createClient("availability/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiAvailability);
                client.Put(request);
            }
        }

        // ------------------------------------------------------------------------------
        // Files

        static string PostResourceFile(string id, ApiResourceFile newResourceFile) {
            using (var client = createClient("resourceFile/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(newResourceFile);
                return client.Post<string>(request);
            }
        }

        static string GetResourceFileName(string id) {
            using (var client = createClient("resourceFile/" + id)) {
                return client.Get<string>(new RestRequest());
            }
        }

        static void DeleteResourceFile(string id) {
            using (var client = createClient("resourceFile/" + id)) {
                client.Delete(new RestRequest());
            }
        }

        static string PostProfileImage(string id, ApiResourceFile newImageFile) {
            using (var client = createClient("profileImage/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(newImageFile);
                return client.Post<string>(request);
            }
        }

        static string GetProfileImageFileName(string id) {
            using (var client = createClient("profileImage/" + id)) {
                return client.Get<string>(new RestRequest());
            }
        }

        static void DeleteProfileImage(string id) {
            using (var client = createClient("profileImage/" + id)) {
                client.Delete(new RestRequest());
            }
        }

        // ------------------------------------------------------------------------------
        // Lookup lists

        static List<string> GetCompetenceList() {
            using (var client = createClient("competenceList")) {
                return client.Get<List<string>>(new RestRequest());
            }
        }

        static List<string> GetCurrencyList() {
            using (var client = createClient("currencyList")) {
                return client.Get<List<string>>(new RestRequest());
            }
        }

        static List<string> GetLanguageList() {
            using (var client = createClient("languageList")) {
                return client.Get<List<string>>(new RestRequest());
            }
        }

        static List<string> GetEducationTypeList() {
            using (var client = createClient("educationTypeList")) {
                return client.Get<List<string>>(new RestRequest());
            }
        }

        // ------------------------------------------------------------------------------
        // Competences, work history, languages, educations

        static List<ApiCompetence> GetResourceCompetences(string id) {
            using (var client = createClient("competence/" + id)) {
                return client.Get<List<ApiCompetence>>(new RestRequest());
            }
        }

        static void PostCompetences(string id, List<ApiCompetence> apiCompetences) {
            using (var client = createClient("competence/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiCompetences);
                client.Post(request);
            }
        }

        static List<ApiWorkHistory> GetResourceWorkHistory(string id) {
            using (var client = createClient("workHistory/" + id)) {
                return client.Get<List<ApiWorkHistory>>(new RestRequest());
            }
        }

        static void PostWorkHistory(string id, List<ApiWorkHistory> apiWorkHistory) {
            using (var client = createClient("workHistory/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiWorkHistory);
                client.Post(request);
            }
        }

        static List<ApiLanguage> GetResourceLanguages(string id) {
            using (var client = createClient("language/" + id)) {
                return client.Get<List<ApiLanguage>>(new RestRequest());
            }
        }

        static void PostLanguages(string id, List<ApiLanguage> apiLanguages) {
            using (var client = createClient("language/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiLanguages);
                client.Post(request);
            }
        }

        static List<ApiEducation> GetResourceEducations(string id) {
            using (var client = createClient("education/" + id)) {
                return client.Get<List<ApiEducation>>(new RestRequest());
            }
        }

        static void PostEducations(string id, List<ApiEducation> apiEducations) {
            using (var client = createClient("education/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiEducations);
                client.Post(request);
            }
        }

        // ------------------------------------------------------------------------------

        private static RestClient createClient(string uri) {
            var client = new RestClient(BASE_URI + uri);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("apikey", API_KEY);
            return client;
        }
    }

    // ===============================================================================================
    // Models
    // ===============================================================================================

    public class ApiAvailability {
        public bool IsAvailable { get; set; }       // Required
        public string AvailableFrom { get; set; }   // Leave empty if IsAvailable == false. Use dd.MM.yyyy format. Sample "20.01.2027"
    }

    public class ApiCompetence {
        public string CompetenceName { get; set; }  // Required. Standard names from GET /competenceList are preferred, but any English name is accepted
        public double WorkExperienceYears { get; set; } // Required
    }

    public class ApiResource {
        public string ID { get; set; }              // Unique id for each resource. Id is created by the API. Leave empty when posting a new resource.
        public string Name { get; set; }            // Required. You can use first name only or initials if you prefer
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Linkedin { get; set; }
        public string Location { get; set; }        // City
        public bool IsAvailable { get; set; }       // Required
        public string AvailableFrom { get; set; }   // Leave empty if IsAvailable == false. Use dd.MM.yyyy format. Sample "20.01.2027"
        public string PriceRequest { get; set; }    // Hourly price, numbers only
        public string Currency { get; set; }        // Currency of PriceRequest from GET /currencyList. Defaults to EUR.
        public string Summary { get; set; }         // Free text summary of the resource
    }

    public class ApiResourceFile {
        public string FileName { get; set; }        // Required. The extension decides the file type.
        public string Base64Data { get; set; }      // Required. Plain base64 string or a data URL (data:...;base64,...)
    }

    public class ApiWorkHistory {
        public string StartDate { get; set; }       // Required. Use dd.MM.yyyy format. Sample "20.01.2023"
        public string EndDate { get; set; }         // Leave empty, if work has not ended. Use dd.MM.yyyy format. Sample "20.01.2023"
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
    }

    public class ApiLanguage {
        public string Language { get; set; }        // Required. Name from GET /languageList, for example "English"
        public int Level { get; set; }              // 1 = Basics, 2 = Intermediate, 3 = Fluent, 4 = Native. 0 removes the language.
    }

    public class ApiEducation {
        public string Type { get; set; }            // Required. Name from GET /educationTypeList
        public string Name { get; set; }            // Required. Degree or course name
        public string Institute { get; set; }       // Required. School, university or training provider
        public string EndDate { get; set; }         // Graduation date in dd.MM.yyyy format, empty if ongoing
    }
}
