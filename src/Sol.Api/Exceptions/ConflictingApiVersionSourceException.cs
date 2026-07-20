namespace Sol.Api.Exceptions;

public class ConflictingApiVersionSourceException(string headerName)
    : Exception($"The API version must be supplied via either the URL segment or the '{headerName}' header, not both.");