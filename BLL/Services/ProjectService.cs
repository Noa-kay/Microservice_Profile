using AutoMapper;
using Microsoft.AspNetCore.Http;
using student_profile.BLL.Interfaces;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IFileService fileService,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(Guid userId)
    {
        var projects = await _projectRepository
            .FindAsync(p => p.UserId == userId);

        return projects.Select(p => _mapper.Map<ProjectDto>(p)).ToList();
    }

    public async Task<ProjectDto?> AddProjectAsync(Guid userId, ProjectDto projectDto, IFormFile? imageFile)
    {
        if (string.IsNullOrWhiteSpace(projectDto.Title))
        {
            throw new ArgumentException("Project title is required.", nameof(projectDto.Title));
        }

        if (!string.IsNullOrWhiteSpace(projectDto.GitHubLink)
            && !Uri.TryCreate(projectDto.GitHubLink, UriKind.Absolute, out _))
        {
            throw new ArgumentException("GitHub link is not a valid URL.", nameof(projectDto.GitHubLink));
        }

        // ודא שהמשתמש קיים
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        string? imageUrl = projectDto.ProjectsImages;

        if (imageFile is not null && imageFile.Length > 0)
        {
            imageUrl = await _fileService.UploadFileAsync(imageFile);
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = projectDto.Title,
            Description = projectDto.Description,
            ProjectName = projectDto.ProjectName,
            GitHubLink = projectDto.GitHubLink,
            ProjectsImages = imageUrl
        };

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project is null)
        {
            return false;
        }

        await _projectRepository.DeleteAsync(project);
        await _projectRepository.SaveChangesAsync();

        return true;
    }

}

