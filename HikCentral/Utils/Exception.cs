namespace HikCentral.Utils;

public class PersonNotFoundException(string message) : Exception(message);
public class PersonInformationNotRetrievedException(string message) : Exception(message);
public class AccessLevelAndAccessGroupNotRetrievedException(string message) : Exception(message);
public class AccessLevelReApplyException(string message) : Exception(message);
public class RecipientsException(string message) : Exception(message);
public class SettingsException(string message) : Exception(message);
public class SenderException(string message) : Exception(message);
public class ApiPathException(string message) : Exception(message);
public class EndpointNullException(string message) : Exception(message);