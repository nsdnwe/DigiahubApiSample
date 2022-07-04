using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

// For more detailed API specification see https://digiahub.com/swagger
//
// nuget Install-Package Newtonsoft.Json
// nuget Install-Package RestSharp

namespace DigiahubApiSample {
    class Program {

        // Replace API_KEY with your api_key OR use "sandbox-mode" for testing.
        // In "sandbox-mode" no changes are saved permanently. Do not send any sensitive data in "sandbox-mode".

        const string API_KEY = "sandbox-mode";

        const string BASE_URI = "https://digiahub.com/api/"; 

        static void Main(string[] args) {

            // ===============================================================================================
            // Post new resource and use new resource's ID in next calls

            var newResource = new ApiResource() {
                Name = "Sample name",
                Email = "sample@email.com",
                IsAvailable = true,
                AvailableFrom = "20.01.2023",
                Phone = "+358405001234"
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
                Location = "Espoo"
            };
            PutResource(resourceID, updatedResource);

            Console.WriteLine("PUT /resource");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var apiAvailability = new ApiAvailability() {
                AvailableFrom = "21.01.2023",
                IsAvailable = true
            };
            SetAvailability(resourceID, apiAvailability);

            Console.WriteLine("PUT /availability");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var apiResourceFile = new ApiResourceFile() {
                Base64Data = "VGhpcyBpcyBhIHNhbXBsZSBmb3IgQmFzZTY0IGNvbnZlcnRpb24u",
                FileName = "sample-cv.txt"
            };
            string newFileName = PostResourceFile(resourceID, apiResourceFile);

            // Sample code how to convert file to Base64 string
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

            var competenceList = GetCompetenceList();

            Console.WriteLine("GET /competenceList");
            Console.WriteLine(JsonConvert.SerializeObject(competenceList, Formatting.Indented));
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

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

            var newWorkHistory = new List<ApiWorkHistory>();
            newWorkHistory.Add(new ApiWorkHistory() { CompanyName = "NSD Consulting Oy", StartDate = "20.01.2010", EndDate = "31.12.2020", JobTitle = "Software developer", JobDescription = "Developing fullstack application for web and mobile." });
            newWorkHistory.Add(new ApiWorkHistory() { CompanyName = "Digia Oyj", StartDate = "01.01.2021", EndDate = "", JobTitle = "System architect", JobDescription = "Design high availability cloud applications." });
            PostWorkHistory(resourceID, newWorkHistory);

            Console.WriteLine("POST /workHistory");
            Console.WriteLine("Done");
            Console.WriteLine("-------------------------------------------------------");

            // ===============================================================================================

            var workHistory = GetResourceWorkHistory(resourceID);

            Console.WriteLine("GET /workHistory");
            Console.WriteLine(JsonConvert.SerializeObject(competences, Formatting.Indented));
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
                var request = new RestRequest();
                var response = client.Get<List<ApiResource>>(request);
                return response;
            }   
        }

        static ApiResource GetResource(string id) {
            using (var client = createClient("resource/" + id)) {
                var request = new RestRequest();
                var response = client.Get<ApiResource>(request);
                return response;
            }
        }

        static string PostResource(ApiResource newResource) {
            using (var client = createClient("resource")) {
                var request = new RestRequest();
                request.AddJsonBody(newResource);
                var response = client.Post<string>(request);
                return response;
            }
        }

        static void PutResource(string id, ApiResource newResource) {
            using (var client = createClient("resource/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(newResource);
                client.Put<string>(request);
            }
        }

        static void DeleteResource(string id) {
            using (var client = createClient("resource/" + id)) {
                var request = new RestRequest();
                client.Delete(request);
            }
        }

        static void SetAvailability(string id, ApiAvailability apiAvailability) {
            using (var client = createClient("availability/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiAvailability);
                client.Put(request);
            }
        }

        static string PostResourceFile(string id, ApiResourceFile newResourceFile) {
            using (var client = createClient("resourceFile/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(newResourceFile);
                var response = client.Post<string>(request);
                return response;
            }
        }

        static string GetResourceFileName(string id) {
            using (var client = createClient("resourceFile/" + id)) {
                var request = new RestRequest();
                string response = client.Get<string>(request);
                return response;
            }
        }

        static List<string> GetCompetenceList() {
            using (var client = createClient("competenceList")) {
                var request = new RestRequest();
                var response = client.Get<List<string>>(request);
                return response;
            }
        }

        static List<ApiCompetence> GetResourceCompetences(string id) {
            using (var client = createClient("competence/" + id)) {
                var request = new RestRequest();
                var response = client.Get<List<ApiCompetence>>(request);
                return response;
            }
        }

        static string PostCompetences(string id, List<ApiCompetence> apiCompetences) {
            using (var client = createClient("competence/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiCompetences);
                var response = client.Post<string>(request);
                return response;
            }
        }

        static List<ApiWorkHistory> GetResourceWorkHistory(string id) {
            using (var client = createClient("workHistory/" + id)) {
                var request = new RestRequest();
                var response = client.Get<List<ApiWorkHistory>>(request);
                return response;
            }
        }

        static string PostWorkHistory(string id, List<ApiWorkHistory> apiWorkHistory) {
            using (var client = createClient("workHistory/" + id)) {
                var request = new RestRequest();
                request.AddJsonBody(apiWorkHistory);
                var response = client.Post<string>(request);
                return response;
            }
        }

        // ------------------------------------------------------------------------------

        private static RestClient createClient(string uri) {
            var client = new RestClient(BASE_URI + uri);
            client.AddDefaultHeader("Content-Type", "application/json");
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
        public string AvailableFrom { get; set; }   // Leave empty if IsAvailable == false. Use dd.MM.yyyy format. Sample "20.01.2023"
    }

    public class ApiCompetence {
        public string CompetenceName { get; set; }  // Required
        public double WorkExperienceYears { get; set; } // Required
    }

    public class ApiResource {
        public string ID { get; set; }              // Unique id for each resource. Id is created by API. Leave empty when posting a new resource.
        public string Name { get; set; }            // Required. You can use first name only or initials if you prefer
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Linkedin { get; set; }
        public string Location { get; set; }        // City 
        public bool IsAvailable { get; set; }       // Required
        public string AvailableFrom { get; set; }   // Leave empty if IsAvailable == false. Use dd.MM.yyyy format. Sample "20.01.2023"
        public string PriceRequest { get; set; }    // €/h
    }

    public class ApiResourceFile {
        public string FileName { get; set; }
        public string Base64Data { get; set; }
    }

    public class ApiWorkHistory {
        public string StartDate { get; set; }       // Required. Use dd.MM.yyyy format. Sample "20.01.2023"
        public string EndDate { get; set; }         // Leave empty, if work has not ended. Use dd.MM.yyyy format. Sample "20.01.2023"
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }

    }
}
