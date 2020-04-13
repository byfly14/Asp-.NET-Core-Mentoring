using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Asp_.NET_MVC_Core_Mentoring_Module1.Controllers;
using Asp_.NET_MVC_Core_Mentoring_Module1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Asp_.NET_Core_Mentoring.UnitTests
{
    public class CategoriesControllersTests
    {
        private CategoriesController _controller;
        private Mock<IDiskImageCacheService> _diskImageCacheService;
        private Mock<IRepository<Categories>> _repository;


        private readonly List<Categories> _categories = new List<Categories>()
        {
            new Categories
            {
                Picture = RandomBufferGenerator.GenerateBufferFromSeed(1024, 1024), CategoryId = 1, CategoryName = "Name1", Description = "Description1", Products = new List<Products>()
            },
            new Categories
            {
                Picture = RandomBufferGenerator.GenerateBufferFromSeed(2048, 2048), CategoryId = 2, CategoryName = "Name2", Description = "Description3", Products = new List<Products>()
            }
        };

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository<Categories>>();
            _repository.Setup(r => r.GetAll()).Returns(() => _categories);
            _repository.Setup(r => r.GetById(It.IsAny<int>())).Returns(() => _categories[0]);
            
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(uof => uof.Repository<Categories>()).Returns(() => _repository.Object);
            unitOfWork.Setup(uof => uof.Context).Returns(() => new FakeDbContext());

            _diskImageCacheService = new Mock<IDiskImageCacheService>();
            _diskImageCacheService.Setup(dis => dis.TryGetImagePath(It.IsAny<int>(), out It.Ref<string>.IsAny)).Returns(() => false);

            _controller = new CategoriesController(unitOfWork.Object, _diskImageCacheService.Object);
        }

        [Test]
        public void CategoriesController_Index_ReturnCorrectModel()
        {
            var result = _controller.Index();

            var model = result.ViewData.Model as IEnumerable<Categories>;
            CollectionAssert.AreEqual(_categories, model);
        }

        [TestCase(1)]
        public void CategoriesController_GetCategoryImage_ReturnCorrectFileStreamResult(int id)
        {
            var result = _controller.GetCategoryImage(id);

            var actualBytes = ReadFully(result.FileStream);
            var expectedBytes = _categories.First(c => c.CategoryId == id).Picture;
            expectedBytes = expectedBytes.Skip(expectedBytes[0] == 0x42 && expectedBytes[1] == 0x4D ? 0 : 78).ToArray();

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [TestCase(1)]
        public void CategoriesController_GetCategoryImageViaAttributeRouting_ReturnCorrectFileStreamResult(int id)
        {
            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "the-mandalorian-4k-2019-tv-show-scaled.jpg");
            _diskImageCacheService.Setup(dis => dis.TryGetImagePath(It.IsAny<int>(), out filePath))
                .Returns(() => true);

            var expectedBytes = File.ReadAllBytes(filePath);

            var result = _controller.GetCategoryImage(id);

            var actualBytes = ReadFully(result.FileStream);

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [TestCase(1)]
        public void CategoriesController_Upload_SuccessfullySaveFile(int id)
        {
            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "the-mandalorian-4k-2019-tv-show-scaled.jpg");
            var expectedBytes = File.ReadAllBytes(filePath);
            
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var file = new FormFile(fs, 0, fs.Length, "temp", "the-mandalorian-4k-2019-tv-show-scaled.jpg");

                _controller.Upload(file, id);

                _repository.Verify(c => c.Update(It.IsAny<Categories>()), Times.Once);
                var actualBytes = _categories.First(c => c.CategoryId == id).Picture;
                CollectionAssert.AreEqual(expectedBytes, actualBytes);
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

    public static class RandomBufferGenerator
    {
        public static byte[] GenerateBufferFromSeed(int maxBufferSize, int size)
        {
            var random = new Random();

            var seedBuffer = new byte[maxBufferSize];

            random.NextBytes(seedBuffer);

            var randomWindow = random.Next(0, size);

            var buffer = new byte[size];

            Buffer.BlockCopy(seedBuffer, randomWindow, buffer, 0, size - randomWindow);
            Buffer.BlockCopy(seedBuffer, 0, buffer, size - randomWindow, randomWindow);

            return buffer;
        }
    }

    public class FakeDbContext : DbContext
    {
        public override int SaveChanges()
        {
            return 0;
        }
    }
}