using Domain.Entities;
using Domain.Exceptions;
using Domain.Ports;
using Domain.Services;
using NSubstitute;

namespace Domain.Tests;

public class RecordVoterTests
{

    readonly IVoterRepository _repository = default!;
    readonly IUnitOfWork _unitOfWork;
    readonly RecordVoterService _service = default!;

    public RecordVoterTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _repository = Substitute.For<IVoterRepository>();
        _service = new RecordVoterService(_repository, _unitOfWork);
    }

    [Fact]
    public void RecordVoterAsync_WithWrongDocument_ThrowsUnderAgeException()
    {
        try
        {
            Voter voter = new("123456", DateTime.Now.AddYears(-17), "COLOMBIA");
            Assert.Fail("Should not get here");
        }
        catch (CoreBusinessException ex)
        {
            Assert.True(ex.Message.Equals("the document requires at least 8 chars"));
        }

    }

    [Fact]
    public async void RecordVoterAsync_WhenVoterIsUnderAge_ThrowsUnderAgeException()
    {
        try
        {
            Voter voter = new("12345678", DateTime.Now.AddYears(-17), "COLOMBIA");
            await _service.RecordVoterAsync(voter);
            Assert.Fail("It shouldn't get to this point");
        }
        catch (UnderAgeException uae)
        {
            Assert.True(uae.Message.Equals("Voter is not 18 years or older"));
        }
    }

    [Fact]
    public async void RecordVoterAsync_WhenVoterIsFromWrongCountry_ThrowsWrongCountryException()
    {
        try
        {
            Voter voter = new("12345678", DateTime.Now.AddYears(-18), "USA");
            await _service.RecordVoterAsync(voter);
            Assert.Fail("It shouldn't get to this point");
        }
        catch (WrongCountryException wce)
        {
            Assert.True(wce.Message.Equals($"Voter is not from Colombia"));
        }
    }

    [Fact]
    public async void RecordVoterAsync_WhenVoterIsOver18AndCorrectCountry_ShouldRecordvoter()
    {
        Voter voter = new("12345678", DateTime.Now.AddYears(-18), "Colombia");
        _repository.SaveVoter(Arg.Any<Voter>()).Returns(voter);
        _unitOfWork.SaveAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var result = await _service.RecordVoterAsync(voter);
        await _repository.Received().SaveVoter(Arg.Any<Voter>());
        await _unitOfWork.Received().SaveAsync(Arg.Any<CancellationToken>());
        Assert.Equal(voter, result);
    }

}
