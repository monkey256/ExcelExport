using System.IO;
namespace tablegen2.layouts
{
    /// <summary>
    /// TreeListItem.xaml 的交互逻辑
    /// </summary>
    public partial class TreeListItem : ListItemBase
    {
        public string FullPath { get; set; }

        public TreeListItem()
        {
            InitializeComponent();
        }

        public void setTreeItemFile(string fullPath)
        {
            FullPath = fullPath;
            txtName.Text = System.IO.Path.GetFileName(fullPath);

            var desc = "未知大小";
            if (File.Exists(fullPath))
            {
                var fi = new FileInfo(fullPath);
                desc = Util.ToFormatLength(fi.Length);
            }
            txtDesc.Text = desc;
        }
    }
}
