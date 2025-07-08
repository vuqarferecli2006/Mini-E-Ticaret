namespace E_Biznes.Application.Shared;

public static class Permission
{
    public static class Category
    {
        public const string MainCategoryCreate = "Category.MainCreate";
        public const string SubCategoryCreate = "Category.SubCreate";
        public const string MainCategoryUpdate = "Category.MainUpdate";
        public const string SubCategoryUpdate = "Category.SubUpdate";
        public const string Delete = "Category.Delete";

        public static List<string> All = new()
        {
            MainCategoryCreate,
            SubCategoryCreate,
            MainCategoryUpdate,
            SubCategoryUpdate,
            Delete
        };
    }

    public static class Role
    {
        public const string Create = "Role.Create";
        public const string Update = "Role.Update";
        public const string Delete = "Role.Delete";
        public const string DeletePermission = "Role.DeletePermission";
        public const string GetAllPermission = "Role.GetAllPermission";
        public const string GetRoleWithPermissions = "Role.GetRoleWithPermissions";


        public static List<string> All = new()
        {
            Create,
            GetAllPermission,
            Update,
            Delete,
            DeletePermission,
            GetRoleWithPermissions
        };
    }

    public static class Account
    {
        public const string AddRole = "Account.AddRole";
        public const string Create = "Account.Create";

        public static List<string> All = new()
        {
            AddRole,
            Create
        };
    }

    public static class Product
    {
        public const string Create = "Product.Create";
        public const string Update = "Product.Update";
        public const string Delete = "Product.Delete";
        public const string GetMy = "Product.GetMy";
        public const string GetAll = "Product.GetAll";
        public const string DeleteProductImage = "Product.DeleteImage";
        public const string AddProductImage = "Product.AddImage";
        public const string AddProductFavourite = "Product.AddFavourite";
        public const string GetAllFavourite = "Product.GetAllFavourite";
        public const string DeleteFavourite = "Product.DeleteFavourite";
        public const string AddProductDisCount = "Product.AddDisCount";
        public const string CancelProductDisCount = "Product.CancelDisCount";

        public static List<string> All = new()
        {
            Create,
            Update,
            Delete,
            GetMy,
            DeleteProductImage,
            AddProductImage,
            AddProductFavourite,
            GetAllFavourite,
            DeleteFavourite,
            GetAll,
            AddProductDisCount,
            CancelProductDisCount
        };
    }

    public static class Order
    {
        public const string Create = "Order.Create";
        public const string GetAll = "Order.GetAll";
        public const string Update = "Order.Update";
        public const string Delete = "Order.Delete";
        public const string GetMy = "Order.GetMy";
        public const string GetDetail = "Order.GetDetail";
        public const string GetMySales = "Order.GetMySales";

        public static List<string> All = new()
        {
            Create,
            GetAll,
            Update,
            Delete,
            GetMy,
            GetMySales,
            GetDetail
        };
    }

    public static class User
    {
        public const string PasswordReset = "User.PasswordReset";
        public const string SendResetEmail = "User.SendResetEmail";
        public const string Create = "User.Create";
        public const string GetAll = "User.GetAll";
        public const string GetById = "User.GetById";
        public const string GetMy = "User.GetMy";

        public static List<string> All = new()
        {
            PasswordReset,
            Create,
            GetAll,
            GetById,
            SendResetEmail,
            GetMy
        };
    }

    public static class Review
    {
        public const string Create = "Review.Create";
        public const string Delete = "Review.Delete";
        public const string GetByProduct = "Review.GetByProduct";
        public const string GetByUser = "Review.GetByUser";

        public static List<string> All = new()
        {
            Create,
            Delete,
            GetByProduct,
            GetByUser
        };
    }

}
