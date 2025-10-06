using Moq;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Dtos.Companies;
using QuartileStore.Commons.Exceptions;
using QuartileStore.Commons.Infrastructure.Transactions;
using QuartileStore.Commons.Services;

namespace QuartileStore.UnitTests.Services;

public class CompanyServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _companyService = new CompanyService(
            _companyRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyDoesNotExist_ShouldCreateCompany()
    {
        var companyCreateDto = new CompanyCreateDto
        {
            Code = 1,
            Name = "Name",
            CountryCode = "USA"
        };
        
        _companyRepositoryMock
            .Setup(r => r.Exists(x => x.Code == companyCreateDto.Code))
            .ReturnsAsync(false);
        
        var result = await _companyService.CreateAsync(companyCreateDto);
        
        Assert.NotNull(result);
        Assert.Equal(companyCreateDto.Code, result.Code);
        Assert.Equal(companyCreateDto.Name, result.Name);
    }
    
    [Fact]
    public async Task CreateAsync_WhenCompanyAlreadyExists_ShouldThrowEntityAlreadyExistsException()
    {
        var companyCreateDto = new CompanyCreateDto
        {
            Code = 1,
            Name = "Name",
            CountryCode = "USA"
        };

        _companyRepositoryMock
            .Setup(r => r.Exists(x => x.Code == companyCreateDto.Code))
            .ReturnsAsync(true);

        var act = () => _companyService.CreateAsync(companyCreateDto);
        
        await Assert.ThrowsAsync<EntityAlreadyExistsException>(act);
        
        _companyRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }
    
    [Fact]
    public async Task GetAsync_WhenCompanyExists_ReturnsCompanyDto()
    {
        const int companyCode = 1;

        var company = new Company
        {
            Code = companyCode,
            Name = "Test",
            CountryCode = "USA"
        };

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(company);

        var result = await _companyService.GetAsync(company.Code);

        Assert.NotNull(result);
        Assert.Equal(company.Code, result.Code);
        Assert.Equal(company.Name, result.Name);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(null as Company);
        
        var act = () => _companyService.GetAsync(companyCode);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(act);
    }

    [Fact]
    public async Task GetAllAsync_WhenCompaniesExist_ShouldReturnCompanyDtoList()
    {
        var companies = new List<Company>
        {
            new() { Code = 1, Name = "Company One", CountryCode = "CO1" },
            new() { Code = 2, Name = "Company Two", CountryCode = "CO2" }
        };

        _companyRepositoryMock
            .Setup(r => r.SelectAllAsync())
            .ReturnsAsync(companies);

        var result = await _companyService.GetAllAsync();

        Assert.NotNull(result);
        var companyDtos = result.ToList();
        Assert.Equal(2, companyDtos.Count);

        var firstCompany = companyDtos.First();
        Assert.Equal(companies.First().Code, firstCompany.Code);
        Assert.Equal(companies.First().Name, firstCompany.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyExists_ShouldUpdatePropertiesAndComplete()
    {
        const int companyCode = 1;

        var company = new Company
        {
            Code = companyCode,
            Name = "Name",
            CountryCode = "USA"
        };

        var companyUpdateDto = new CompanyUpdateDto
        {
            Name = "Updated Name",
            CountryCode = "Updated USA"
        };

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(company);

        await _companyService.UpdateAsync(companyCode, companyUpdateDto);

        Assert.Equal(company.Name, companyUpdateDto.Name);
        Assert.Equal(company.CountryCode, companyUpdateDto.CountryCode);

        _unitOfWorkMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyDoesNotExists_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;

        var act = () => _companyService.UpdateAsync(companyCode, new CompanyUpdateDto());

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _companyRepositoryMock.Verify(r => r.Delete(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyExistsAndHasNoStores_ShouldDeleteCompany()
    {
        const int companyCode = 1;

        var company = new Company
        {
            Code = companyCode,
            Name = "Name",
            CountryCode = "USA"
        };

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(company);

        _companyRepositoryMock
            .Setup(r => r.HasStoresAsync(company.Id))
            .ReturnsAsync(false);

        await _companyService.DeleteAsync(companyCode);

        _companyRepositoryMock.Verify(r => r.Delete(It.IsAny<Company>()), Times.Once);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyDoesNotExists_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;

        var act = () => _companyService.DeleteAsync(companyCode);

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _companyRepositoryMock.Verify(r => r.Delete(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyHasStores_ShouldThrowDeleteConflictException()
    {
        const int companyCode = 1;

        var company = new Company
        {
            Code = companyCode,
            Name = "Test",
            CountryCode = "BRA"
        };

        _companyRepositoryMock
            .Setup(r => r.SelectOneByAsync(x => x.Code == companyCode))
            .ReturnsAsync(company);

        _companyRepositoryMock
            .Setup(r => r.HasStoresAsync(company.Id))
            .ReturnsAsync(true);

        var act = () => _companyService.DeleteAsync(company.Code);

        await Assert.ThrowsAsync<DeleteConflictException>(act);

        _companyRepositoryMock.Verify(r => r.Delete(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }
}