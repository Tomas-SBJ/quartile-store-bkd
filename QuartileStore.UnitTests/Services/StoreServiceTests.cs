using Moq;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Dtos.Stores;
using QuartileStore.Commons.Exceptions;
using QuartileStore.Commons.Infrastructure.Transactions;
using QuartileStore.Commons.Services;

namespace QuartileStore.UnitTests.Services;

public class StoreServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IStoreRepository> _storeRepositoryMock;
    private readonly StoreService _storeService;

    public StoreServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _storeRepositoryMock = new Mock<IStoreRepository>();

        _storeService = new StoreService(
            _storeRepositoryMock.Object,
            _companyRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyDoesNotExist_ShouldCreateCompany()
    {
        const int companyCode = 1;

        var company = new Company
        {
            Code = companyCode,
            Name = "Name",
            CountryCode = "USA"
        };

        var storeCreateDto = new StoreCreateDto()
        {
            Code = 1,
            Name = "Name",
            Address = "Brazil"
        };

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(company);

        _storeRepositoryMock
            .Setup(r => r.Exists(x => x.Code == storeCreateDto.Code && x.CompanyId == company.Id))
            .ReturnsAsync(false);

        var result = await _storeService.CreateAsync(companyCode, storeCreateDto);

        Assert.NotNull(result);
        Assert.Equal(storeCreateDto.Code, result.Code);
        Assert.Equal(storeCreateDto.Name, result.Name);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyDoesNotExists_ShouldThrowEntityAlreadyExistsException()
    {
        const int companyCode = 1;

        var storeCreateDto = new StoreCreateDto()
        {
            Code = 1,
            Name = "Name",
            Address = "Brazil"
        };

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(null as Company);

        var act = () => _storeService.CreateAsync(companyCode, storeCreateDto);

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _companyRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenStoreAlreadyExists_ShouldThrowEntityAlreadyExistsException()
    {
        const int companyCode = 1;

        var company = new Company
        {
            Code = companyCode,
            Name = "Test Company",
            CountryCode = "BRA"
        };

        var storeCreateDto = new StoreCreateDto()
        {
            Code = 1,
            Name = "Name",
            Address = "Brazil"
        };

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(company);

        _storeRepositoryMock
            .Setup(r => r.Exists(x => x.Code == storeCreateDto.Code && x.CompanyId == company.Id))
            .ReturnsAsync(true);

        var act = () => _storeService.CreateAsync(companyCode, storeCreateDto);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(act);

        _companyRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetAsync_WhenStoreExists_ReturnsStoreDto()
    {
        const int companyCode = 1;
        const int storeCode = 1;

        var store = new Store
        {
            Code = storeCode,
            Name = "Test",
            Address = "Brazil",
            Company = new Company
            {
                Code = companyCode,
                Name = "Test",
                CountryCode = "BRA"
            }
        };

        _storeRepositoryMock
            .Setup(r => r.SelectOneWithCompanyAsync(storeCode, companyCode))
            .ReturnsAsync(store);

        var result = await _storeService.GetAsync(storeCode, companyCode);

        Assert.NotNull(result);
        Assert.Equal(store.Code, result.Code);
        Assert.Equal(store.Name, result.Name);
    }

    [Fact]
    public async Task GetAsync_WhenStoreDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;
        const int storeCode = 1;

        _storeRepositoryMock
            .Setup(r => r.SelectOneWithCompanyAsync(storeCode, companyCode))
            .ReturnsAsync(null as Store);

        var act = () => _storeService.GetAsync(storeCode, companyCode);

        await Assert.ThrowsAsync<EntityNotFoundException>(act);
    }

    [Fact]
    public async Task GetAllAsync_WhenStoresExist_ShouldReturnStoreDtoList()
    {
        const int companyCode = 1;

        var stores = new List<Store>
        {
            new()
            {
                Code = 1,
                Name = "Store One",
                Address = "Brazil",
                Company = new Company
                {
                    Code = companyCode,
                    Name = "Test",
                    CountryCode = "USA"
                }
            },
            new()
            {
                Code = 2,
                Name = "Store Two",
                Address = "United States",
                Company = new Company
                {
                    Code = companyCode,
                    Name = "Test",
                    CountryCode = "USA"
                }
            }
        };

        _storeRepositoryMock
            .Setup(r => r.SelectAllByCompanyCodeAsync(companyCode))
            .ReturnsAsync(stores);

        var result = await _storeService.GetAllAsync(companyCode);

        Assert.NotNull(result);
        var storeDtos = result.ToList();
        Assert.Equal(2, storeDtos.Count);

        var firstStore = storeDtos.First();
        Assert.Equal(stores.First().Code, firstStore.Code);
        Assert.Equal(stores.First().Name, firstStore.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenStoreExists_ShouldUpdatePropertiesAndComplete()
    {
        const int companyCode = 1;
        const int storeCode = 1;

        var store = new Store
        {
            Code = storeCode,
            Name = "Name",
            Address = "Brazil",
            Company = new Company
            {
                Code = companyCode,
                Name = "Name",
                CountryCode = "USA"
            }
        };

        var storeUpdateDto = new StoreUpdateDto
        {
            Name = "Updated Name",
            Address = "Updated Brazil"
        };

        _storeRepositoryMock
            .Setup(r => r.SelectOneWithCompanyAsync(storeCode, companyCode))
            .ReturnsAsync(store);

        await _storeService.UpdateAsync(storeCode, companyCode, storeUpdateDto);

        Assert.Equal(store.Name, storeUpdateDto.Name);
        Assert.Equal(store.Address, storeUpdateDto.Address);

        _unitOfWorkMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenStoreDoesNotExists_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;
        const int storeCode = 1;

        var act = () => _storeService.UpdateAsync(storeCode, companyCode, new StoreUpdateDto());

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _storeRepositoryMock.Verify(r => r.Delete(It.IsAny<Store>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenStoreExistsAndHasNoProducts_ShouldDeleteStore()
    {
        const int companyCode = 1;
        const int storeCode = 1;

        var store = new Store
        {
            Code = storeCode,
            Name = "Name",
            Address = "Brazil"
        };

        _storeRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == storeCode && x.Company.Code == companyCode))
            .ReturnsAsync(store);

        _storeRepositoryMock
            .Setup(r => r.HasProductsAsync(store.Id))
            .ReturnsAsync(false);

        await _storeService.DeleteAsync(storeCode, companyCode);

        _storeRepositoryMock.Verify(r => r.Delete(It.IsAny<Store>()), Times.Once);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenStoreDoesNotExists_ShouldThrowEntityNotFoundException()
    {
        const int storeCode = 1;
        const int companyCode = 1;

        var act = () => _storeService.DeleteAsync(storeCode, companyCode);

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _storeRepositoryMock.Verify(r => r.Delete(It.IsAny<Store>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenStoreHasProducts_ShouldThrowDeleteConflictException()
    {
        const int storeCode = 1;
        const int companyCode = 1;

        var store = new Store
        {
            Code = storeCode,
            Name = "Test",
            Address = "Brazil"
        };

        _storeRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == storeCode && x.Company.Code == companyCode))
            .ReturnsAsync(store);

        _storeRepositoryMock
            .Setup(r => r.HasProductsAsync(store.Id))
            .ReturnsAsync(true);

        var act = () => _storeService.DeleteAsync(storeCode, companyCode);

        await Assert.ThrowsAsync<DeleteConflictException>(act);

        _storeRepositoryMock.Verify(r => r.Delete(It.IsAny<Store>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }
}