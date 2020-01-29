class PageUtils
{
    public static ConfirmDelete(e: Event): boolean
    {
        if (!confirm('This item will be permanently deleted. This action cannot be undone.\n\nAre you sure?'))
        {
            e.preventDefault();
            return false;
        }
        return true;
    }
}