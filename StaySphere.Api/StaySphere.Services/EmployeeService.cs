﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaySphere.Domain.DTOs.Employee;
using StaySphere.Domain.Entities;
using StaySphere.Domain.Exeptions;
using StaySphere.Domain.Interfaces.Services;
using StaySphere.Domain.Pagination;
using StaySphere.Domain.ResourceParameters;
using StaySphere.Infrastructure.Persistence;

namespace StaySphere.Services
{
    public class EmployeeService : IEmployeeService
    {
        public readonly IMapper _mapper;
        public readonly StaySphereDbContext _context;
        public readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IMapper mapper, StaySphereDbContext context, ILogger<EmployeeService> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaginatedList<EmployeeDto>> GetEmployees(EmployeeResourceParameters employeeResourceParameters)
        {
            var query = _context.Employees.AsQueryable();

            if (employeeResourceParameters.PositionId is not null)
            {
                query = query.Where(x => x.PositionId == employeeResourceParameters.PositionId);
            }

            if (employeeResourceParameters.Salary is not null)
            {
                query = query.Where(x => x.Salary == employeeResourceParameters.Salary);
            }

            if (employeeResourceParameters.SalaryLessThan is not null)
            {
                query = query.Where(x => x.Salary < employeeResourceParameters.SalaryLessThan);
            }

            if (employeeResourceParameters.SalaryGreaterThan is not null)
            {
                query = query.Where(x => x.Salary > employeeResourceParameters.SalaryGreaterThan);
            }

            if (!string.IsNullOrEmpty(employeeResourceParameters.OrderBy))
            {
                query = employeeResourceParameters.OrderBy.ToLowerInvariant() switch
                {
                    "FirstName" => query.OrderBy(x => x.FirstName),
                    "FirstNamedesc" => query.OrderByDescending(x => x.FirstName),
                    "LastName" => query.OrderBy(x => x.LastName),
                    "LastNamedesc" => query.OrderByDescending(x => x.LastName),
                    "Salary" => query.OrderBy(x => x.Salary),
                    "Salarydesc" => query.OrderByDescending(x => x.Salary),
                    "PositionId" => query.OrderBy(x => x.PositionId),
                    "PositionIddesc" => query.OrderByDescending(x => x.PositionId),
                    _ => query.OrderBy(x => x.FirstName),
                };
            }

            var employees = await query.ToPaginatedListAsync(employeeResourceParameters.PageSize, employeeResourceParameters.PageNumber);
            var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);

            return new PaginatedList<EmployeeDto>(employeeDtos, employees.TotalCount, employees.CurrentPage, employees.PageSize);
        }

        public async Task<EmployeeDto?> GetEmployeeById(int id)
        {
            var employees = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employees is null)
            {
                throw new EntityNotFoundException($"Entity with id {id} not found!");
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employees);
            return employeeDto;
        }

        public async Task<EmployeeDto> CreateEmployee(EmployeeForCreateDto employeeForCreateDto)
        {
            var employeeEntity = _mapper.Map<Employee>(employeeForCreateDto);

            _context.Add(employeeEntity);
            await _context.SaveChangesAsync();

            var employeeDto = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeDto;
        }

        public async Task UpdateEmployee(EmployeeForUpdateDto employeeForUpdateDto)
        {
            var employeeEntity = _mapper.Map<Employee>(employeeForUpdateDto);

            _context.Add(employeeEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee is not null)
                _context.Remove(employee);

            await _context.SaveChangesAsync();
        }
    }
}
