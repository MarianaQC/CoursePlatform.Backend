namespace CoursePlatform.Application.Common.Errors;

public static class DomainErrors
{
    public static class Course
    {
        public const string NotFound = "El curso no fue encontrado.";
        public const string AlreadyDeleted = "El curso ya ha sido eliminado.";
        public const string CannotPublish = "No se puede publicar un curso sin lecciones activas.";
        public const string TitleRequired = "El título del curso es obligatorio.";
        public const string TitleTooLong = "El título del curso no puede exceder los 200 caracteres.";
    }

    public static class Lesson
    {
        public const string NotFound = "La lección no fue encontrada.";
        public const string AlreadyDeleted = "La lección ya ha sido eliminada.";
        public const string DuplicateOrder = "Ya existe una lección con ese orden en este curso.";
        public const string InvalidOrder = "El orden debe ser mayor a 0.";
        public const string TitleRequired = "El título de la lección es obligatorio.";
        public const string TitleTooLong = "El título de la lección no puede exceder los 200 caracteres.";
        public const string CourseIdRequired = "El ID del curso es obligatorio.";
        public const string CourseNotFound = "El curso asociado no fue encontrado.";
        public const string CannotMoveUp = "La lección ya está en la primera posición.";
        public const string CannotMoveDown = "La lección ya está en la última posición.";
    }

    public static class Auth
    {
        public const string InvalidCredentials = "Email o contraseña incorrectos.";
        public const string UserAlreadyExists = "Ya existe un usuario con ese email.";
        public const string RegistrationFailed = "Error al registrar el usuario.";
        public const string EmailRequired = "El email es obligatorio.";
        public const string InvalidEmail = "El formato del email no es válido.";
        public const string PasswordRequired = "La contraseña es obligatoria.";
        public const string PasswordTooShort = "La contraseña debe tener al menos 6 caracteres.";
        public const string FirstNameRequired = "El nombre es obligatorio.";
        public const string LastNameRequired = "El apellido es obligatorio.";
    }
}