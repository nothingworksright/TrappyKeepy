using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    public class UserService : IUserService
    {
        private string connectionString;

        public UserService()
        {
            this.connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        }

        public UserService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<UserServiceResponse> Create(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested new user was not defined.";
                return response;
            }
            if (request.Item.Name is null || request.Item.Email is null || request.Item.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name, Email, and Password are required to create a user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Received a UserDto from the controller. Turn that into a User.
                    var newUser = new Domain.User()
                    {
                        Name = request.Item.Name,
                        Password = request.Item.Password, // Plaintext password here from the request Dto.
                        Email = request.Item.Email,
                        DateCreated = DateTime.Now
                    };
                    var existingNameCount = await unitOfWork.UserRepository
                        .CountByColumnValue("name", newUser.Name);
                    if (existingNameCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new user name already in use.";
                        return response;
                    }
                    var existingEmailCount = await unitOfWork.UserRepository
                        .CountByColumnValue("email", newUser.Email);
                    if (existingEmailCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new user email already in use.";
                        return response;
                    }
                    var newId = await unitOfWork.UserRepository.Create(newUser);
                    unitOfWork.Commit();

                    // Pass a UserDto back to the controller.
                    response.Item = new Domain.UserDto()
                    {
                        Id = newId // Id from the database insert.
                    };
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> ReadAll(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    var userList = await unitOfWork.UserRepository.ReadAll();
                    unitOfWork.Commit();

                    // Pass userDto objects back to the controller.
                    var userDtos = new List<Domain.UserDto>();
                    foreach(Domain.User user in userList)
                    {
                        var userDto = new Domain.UserDto()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            // Do not include the salted/hashed password.
                            Email = user.Email,
                            DateCreated = user.DateCreated,
                            DateActivated = user.DateActivated,
                            DateLastLogin = user.DateLastLogin
                        };
                        userDtos.Add(userDto);
                    }
                    response.List = userDtos;
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> ReadById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user Id was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    var user = await unitOfWork.UserRepository.ReadById((Guid)request.Id);
                    unitOfWork.Commit();

                    // TODO: Get User objects from unitOfWork, convert to Dto objects for the service response.
                    // TODO: Don't include the password in the Dto.


                    var userDto = new Domain.UserDto()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        // Do not include the salted/hashed password.
                        Email = user.Email,
                        DateCreated = user.DateCreated,
                        DateActivated = user.DateActivated,
                        DateLastLogin = user.DateLastLogin
                    };
                    response.Item = userDto;
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> UpdateById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user for update was not defined.";
                return response;
            }
            if (request.Item.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user id for update was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify that the user exists first?
                    // Received a UserDto from the controller. Turn that into a User.
                    var dto = request.Item;
                    var updatee = new Domain.User();
                    if (dto.Id is not null) updatee.Id = (Guid)dto.Id;
                    if (dto.Name is not null) updatee.Name = dto.Name;
                    if (dto.Email is not null) updatee.Email = dto.Email;
                    if (dto.DateActivated is not null) updatee.DateActivated = dto.DateActivated;
                    if (dto.DateLastLogin is not null) updatee.DateLastLogin = dto.DateLastLogin;

                    // The updater updates the updatee.
                    var successful = await unitOfWork.UserRepository.UpdateById(updatee);
                    unitOfWork.Commit();
                    if (successful)
                    {
                        response.Outcome = OutcomeType.Success;
                    }
                    else
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not updated.";
                    }
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> UpdatePasswordById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user for update was not defined.";
                return response;
            }
            if (request.Item.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user id for update was not defined.";
                return response;
            }
            if (request.Item.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested new user password was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify that the user exists first?
                    // Received a UserDto from the controller. Turn that into a User.
                    var dto = request.Item;
                    var updatee = new Domain.User();
                    if (dto.Id is not null) updatee.Id = (Guid)dto.Id;
                    if (dto.Password is not null) updatee.Password = dto.Password;

                    // The updater updates the updatee.
                    var successful = await unitOfWork.UserRepository.UpdatePasswordById(updatee);
                    unitOfWork.Commit();
                    if (successful)
                    {
                        response.Outcome = OutcomeType.Success;
                    }
                    else
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User password was not updated.";
                    }
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> DeleteById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user Id was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify that the user exists first?
                    var successful = await unitOfWork.UserRepository.DeleteById((Guid)request.Id);
                    unitOfWork.Commit();
                    if (successful)
                    {
                        response.Outcome = OutcomeType.Success;
                    }
                    else
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not deleted.";
                    }
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }
    }
}