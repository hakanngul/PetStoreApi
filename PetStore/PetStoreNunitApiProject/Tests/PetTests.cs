﻿using System.Net;

namespace PetStoreNunitApiProject.Tests
{
    [TestFixture]
    public class PetTests : BaseTest
    {
        private readonly IRestFactory _restFactory = new RestFactory(new RestBuilder(new RestLibrary()));



        private Pets GetPet(long id, string name, PetStatus status)
        {
            return new Pets
            {
                Id = id,
                Name = name,
                Status = status,
                Category = new Category
                {
                    Id = 6786586576578568000,
                    Name = "denemeCategory"
                },
                PhotoUrls = new List<string> { "resim1.jpg" },
                Tags = new List<Tag>
                {
                    new Tag { Id = 4564565476456, Name = "tagXXXX" }
                },
            };
        }


        [Test, Order(1)]
        public async Task AddPet()
        {

            var newPet = GetPet(1234567891, "Eragoln", PetStatus.available);
            var result = await _restFactory.Create()
               .WithRequest(Urls.CreatePet)
               .WithHeader("Authorization", "Bearer " + accessToken)
               .WithBody(newPet)
               .WithPostResponse();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var response = JsonConvert.DeserializeObject<Pets>(result.Content);

            Assert.Multiple(() =>
            {
                Assert.That(response?.Id, Is.EqualTo(newPet.Id));
                Assert.That(response?.Name, Is.EqualTo(newPet.Name));
                Assert.That(response?.Status, Is.EqualTo(newPet.Status));
                Assert.That(response?.Tags.Count, Is.EqualTo(newPet.Tags.Count));
                Assert.That(response?.Category.Id, Is.EqualTo(newPet.Category.Id));
                Assert.That(response?.Category.Name, Is.EqualTo(newPet.Category.Name));
                Assert.That(response?.PhotoUrls.Count, Is.EqualTo(newPet.PhotoUrls.Count));
            });
        }

        [Test, Order(2)]
        public async Task GetPetById()
        {
            var newPet = GetPet(1234567891, "Eragoln", PetStatus.available);

            var result = await _restFactory.Create()
               .WithRequest(Urls.GetPetById)
               .WithUrlSegment("petId", "1234567891")
               .WithHeader("Authorization", "Bearer " + accessToken)
               .WithBody(newPet)
               .WithGetResponse();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var response = JsonConvert.DeserializeObject<Pets>(result.Content);
            Assert.Multiple(() =>
            {
                Assert.That(response?.Id, Is.EqualTo(newPet.Id));
                Assert.That(response?.Name, Is.EqualTo(newPet.Name));
                Assert.That(response?.Status, Is.EqualTo(newPet.Status));
                Assert.That(response?.Category.Id, Is.EqualTo(newPet.Category.Id));
                Assert.That(response?.Category.Name, Is.EqualTo(newPet.Category.Name));
                Assert.That(response?.PhotoUrls.Count, Is.EqualTo(newPet.PhotoUrls.Count));
            });
        }

        [Test, Order(3)]
        public async Task UpdatePet()
        {
            var newPet = GetPet(1234567891, "EragolnXx", PetStatus.pending);

            var result = await _restFactory.Create()
                .WithRequest(Urls.UpdatePet)
                .WithHeader("Authorization", "Bearer " + accessToken)
                .WithBody(newPet)
                .WithPutResponse();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var response = JsonConvert.DeserializeObject<Pets>(result.Content);

            Assert.Multiple(() =>
            {
                Assert.That(response?.Id, Is.EqualTo(newPet.Id));
                Assert.That(response?.Name, Is.EqualTo(newPet.Name));
                Assert.That(response?.Status, Is.EqualTo(newPet.Status));
                Assert.That(response?.Category.Id, Is.EqualTo(newPet.Category.Id));
                Assert.That(response?.Category.Name, Is.EqualTo(newPet.Category.Name));
                Assert.That(response?.PhotoUrls.Count, Is.EqualTo(newPet.PhotoUrls.Count));
            });

        }



        [Test, Order(4)]
        [TestCase("available")]
        [TestCase("pending")]
        [TestCase("sold")]
        public async Task GetPetByStatus(string type)
        {

            var result = await _restFactory.Create()
                .WithRequest(Urls.FindByStatus)
                .WithHeader("Authorization", "Bearer " + accessToken)
                .WithQueryParameter("status", type)
                .WithGetResponse();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var response = JsonConvert.DeserializeObject<List<Pets>>(result.Content);

            Assert.That(response, Is.Not.Empty);

        }

        [Test, Order(5)]
        public async Task UpdatePetById()
        {
            var newPet = GetPet(1234567891, "EragolnXx", PetStatus.pending);

            var result = await _restFactory.Create()
                .WithRequest(Urls.UpdatePetById)
                .WithHeader("Authorization", "Bearer " + accessToken)
                .WithUrlSegment("petId", newPet.Id.ToString())
                .WithParameter("petId", newPet.Id.ToString())
                .WithParameter("name", "TestNewNameByHakan")
                .WithParameter("status", "pending")
                .WithPostResponse();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var response = JsonConvert.DeserializeObject<PetResponse>(result.Content);

            Assert.Multiple(() =>
            {
                Assert.That(response?.Code, Is.EqualTo(200));
                Assert.That(response?.Type, Is.Not.Null);
                Assert.That(response?.Message, Does.Contain(newPet.Id.ToString()));
            });

        }

        [Test, Order(6)]
        public async Task DeletePet()
        {
            var newPet = GetPet(1234567891, "EragolnXx", PetStatus.pending);

            var response = await _restFactory.Create()
                .WithRequest(Urls.DeletePetById)
                .WithHeader("Authorization", "Bearer " + accessToken)
                .WithBody(newPet)
                .WithUrlSegment("petId", newPet.Id.ToString())
                .WithDelete<PetResponse>();


            Assert.Multiple(() =>
            {
                Assert.That(response?.Code, Is.EqualTo(200));
                Assert.That(response?.Type, Is.Not.Null);
                Assert.That(response?.Message, Does.Contain(newPet.Id.ToString()));
            });

        }
    }
}
