using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;


using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

using MapControlApplication7.attributeForm;
using MapControlApplication7.User;
using MapControlApplication7.layout;
using MapControlApplication7.search;
using MapControlApplication7.person;
using MapControlApplication7.farmInfo;

namespace MapControlApplication7
{
    public sealed partial class MainForm : Form
    {
        #region class private members
        private IMapControl3 m_mapControl = null;
        private string m_mapDocumentName = string.Empty;
        private ILayer m_layerSelected = null;
        public Form par = null;
        #endregion

        private bool once = true;
        private IPageLayoutControl3 m_PageLayControl = null;

        #region class constructor
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            //get the MapControl
            m_mapControl = (IMapControl3)axMapControl1.Object;

            m_PageLayControl = (IPageLayoutControl3)axPageLayoutControl1.Object;
            //disable the Save menu (since there is no document yet)
            menuSaveDoc.Enabled = false;


            toolStripComboBox1.ComboBox.Items.Clear();
            toolStripComboBox1.ComboBox.Items.Add("");
            toolStripComboBox1.ComboBox.Items.Add("Legend");
            toolStripComboBox1.ComboBox.Items.Add("MarkerNorthArrow");
            toolStripComboBox1.ComboBox.Items.Add("ScaleLine");
            toolStripComboBox1.ComboBox.Items.Add("ScaleText");
           
        }

        #region Main Menu event handlers
        private void menuNewDoc_Click(object sender, EventArgs e)
        {
            //execute New Document command
            ICommand command = new CreateNewDocument();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuOpenDoc_Click(object sender, EventArgs e)
        {
            //execute Open Document command
            ICommand command = new ControlsOpenDocCommandClass();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuSaveDoc_Click(object sender, EventArgs e)
        {
            //execute Save Document command
            if (m_mapControl.CheckMxFile(m_mapDocumentName))
            {
                //create a new instance of a MapDocument
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(m_mapDocumentName, string.Empty);

                //Make sure that the MapDocument is not readonly
                if (mapDoc.get_IsReadOnly(m_mapDocumentName))
                {
                    MessageBox.Show("Map document is read only!");
                    mapDoc.Close();
                    return;
                }

                //Replace its contents with the current map
                mapDoc.ReplaceContents((IMxdContents)m_mapControl.Map);

                //save the MapDocument in order to persist it
                mapDoc.Save(mapDoc.UsesRelativePaths, false);

                //close the MapDocument
                mapDoc.Close();
            }
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            //execute SaveAs Document command
            ICommand command = new ControlsSaveAsDocCommandClass();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuExitApp_Click(object sender, EventArgs e)
        {
            //exit the application
            Application.Exit();
        }
        #endregion

        //listen to MapReplaced evant in order to update the statusbar and the Save menu
        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            //get the current document name from the MapControl
            m_mapDocumentName = m_mapControl.DocumentFilename;

            //if there is no MapDocument, diable the Save menu and clear the statusbar
            if (m_mapDocumentName == string.Empty)
            {
                menuSaveDoc.Enabled = false;
                statusBarXY.Text = string.Empty;
            }
            else
            {
                //enable the Save manu and write the doc name to the statusbar
                menuSaveDoc.Enabled = true;
                statusBarXY.Text = System.IO.Path.GetFileName(m_mapDocumentName);
            }

            //��������ͼ��ӥ�۴���
            if (once)
            {

                CopyAndOverwriteMap();
                once = false;
            }

            CopyAndOverwriteLayoutMap();
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            statusBarXY.Text = string.Format("{0}, {1}  {2}", e.mapX.ToString("#######.##"), e.mapY.ToString("#######.##"), axMapControl1.MapUnits.ToString().Substring(4));
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {

        }

        private void newPolygonFeatureClsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new Command2();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();

        }

        private void newPolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new Tool1();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;

        }

        private void findSpatialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new Command1();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();

        }

        private void newPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new ToolPoint();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;
        }

        private void newPointFeatureClsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new CommandPoint();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void selectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ICommand command = new selectTool();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;
        }

        private void selectAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void addFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new readFileTool();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;
        }

        private void polygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new selectAddTool();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;
        }

        private void pointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new selectAddByRectangle();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;
        }

        private void lineStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new SelectByCircle();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;
        }

        private void exportToshpToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Form a = new Form1(m_mapControl.Object);
            a.Show();
            a.Activate();
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            esriTOCControlItem type =
                         esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap basicMap = null;
            ILayer layer = null;
            object unk = null, data = null;
            axTOCControl1.GetSelectedItem(ref type, ref basicMap,ref layer, ref unk, ref data);
            //�統ǰѡ��������Ϊͼ���������Ҽ�
            if (type == esriTOCControlItem.esriTOCControlItemLayer
                && layer != null
                && e.button == 2)
            {
                //�洢��ǰѡ��ͼ��
                m_layerSelected = layer;
                //�����Ҽ��˵�
                contextMenuStrip1.Show(axTOCControl1, e.x, e.y);
            }


        }

        private void createRasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new CreateRaster(m_layerSelected);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        
        }

        private void axLicenseControl1_Enter(object sender, EventArgs e)
        {

        }

        private void rasterRenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new Command3(m_layerSelected);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void identifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new identifyRaster(m_layerSelected);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
            axMapControl1.CurrentTool = (ITool)command;
        }

        private void exacteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void staticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new staticias(m_layerSelected);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form a = new exacteEqualLine(m_mapControl.Object);
            a.Show();
            a.Activate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.par.Close();
                
              
        }


        private void CopyAndOverwriteMap()
        {
            //�½����󿽱���
            IObjectCopy objCopy = new ObjectCopyClass();
            //�������ͼ����
            object fromMap = axMapControl1.Map;
            //���ӥ�۵�ͼ����
            object objMap = axMapControl2.Map;
            //������ͼ������ӥ�۵�ͼ
            objCopy.Overwrite(fromMap, ref objMap);
            //����ӥ�۵�ͼ��ΧΪ��ͼȫ�ַ�Χ
            axMapControl2.Extent = axMapControl2.FullExtent;
            //ˢ��ӥ�۴���
            axMapControl2.Refresh();
        }



        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //�½�һ������Ԫ�ض���
            IElement ele = new RectangleElementClass();
            //��������Ϊ����ͼ����ʾ��Χ
            ele.Geometry = axMapControl1.Extent;
            //�½�һ���������ʽ����
            IFillSymbol symbol = new SimpleFillSymbolClass();
            //�½�һ��RGB��ɫ����
            IRgbColor clr = new RgbColorClass();
            //���������ʽ�������ɫΪ��ɫ��͸��
            clr.NullColor = true;
            clr.Transparency = 0;
            symbol.Color = clr;
            //�½�һ������ʽ����
            ILineSymbol linSymbol = new SimpleLineSymbolClass();
            //���������ʽ����ı߿�Ϊ��ɫ
            IRgbColor linClr = new RgbColorClass();
            linClr.Red = 255;
            linSymbol.Color = linClr;
            symbol.Outline = linSymbol;
            //�������ʽ���������ƾ���Ԫ��
            ((IFillShapeElement)ele).Symbol = symbol;
            //ɾ��ӥ���е�����Ԫ��
            axMapControl2.ActiveView.GraphicsContainer.DeleteAllElements();
            //������Ԫ����ӵ�ӥ���У����ֲ�ˢ��
            axMapControl2.ActiveView.GraphicsContainer.AddElement(ele, 0);
            axMapControl2.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void ReLoadLayersToHawkEye()
        {
            //IMap oldMap = axMapControl2.Map; //Release original map
            //for (int i = 1; i <= axMapControl2.LayerCount; i++)
            //{
            //    axMapControl1.AddLayer(axMapControl2.get_Layer(axMapControl2.LayerCount - i));
            //}
 
            IObjectCopy objCopy = new ObjectCopyClass();
            object fromMap = axMapControl2.Map;
            //���ӥ�۵�ͼ����
            object objMap = axMapControl1.Map;
            //������ͼ������ӥ�۵�ͼ
            objCopy.Overwrite(fromMap, ref objMap);

            axMapControl1.Refresh();
        }

        private void axMapControl1_OnFullExtentUpdated(object sender, IMapControlEvents2_OnFullExtentUpdatedEvent e)
        {
            //��������ͼ��ӥ�۴���
         
            CopyAndOverwriteMap();

            CopyAndOverwriteLayoutMap();
        }

        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            axMapControl2.ActiveView.GraphicsContainer.DeleteAllElements();
            ReLoadLayersToHawkEye();
            //ӥ���н������ƾ��ο�



            IEnvelope env = axMapControl2.TrackRectangle();

            //�����ο�����Ϊ����ͼ����ʾ��Χ��������ӥ��
            axMapControl1.Extent = env;
            axMapControl1.Refresh();
        }














        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                axToolbarControl1.SetBuddyControl(axMapControl1.Object);
            }
            else
            {
                axToolbarControl1.SetBuddyControl(axPageLayoutControl1.Object);
            }
        }



        private void CopyAndOverwriteLayoutMap()
        {
            //�½����󿽱���
            IObjectCopy objCopy = new ObjectCopyClass();
            //������ض���
            Object fromMap = axMapControl1.Map;
            //���ҳ�沼�ֵ�ͼ����
            Object objMap = axPageLayoutControl1.ActiveView.FocusMap;
            //������ͼ������ҳ�沼�ֵ�ͼ
            objCopy.Overwrite(fromMap, ref objMap);
            //����ҳ�沼�ֵ�ͼ��ΧΪ��ͼȫ�ַ�Χ
            axPageLayoutControl1.Extent = axPageLayoutControl1.FullExtent;
        }

        private void addGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new LayoutAddGrid(m_PageLayControl);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void deleteGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new clearGrid(m_PageLayControl);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void shaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new shadow(m_PageLayControl);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void borderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new border(m_PageLayControl);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new backgroud(m_PageLayControl);
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void toolStripComboBox1_TextChanged(object sender, EventArgs e)
        {
            string type = "esriCarto." + toolStripComboBox1.ComboBox.Text;
            if (tabControl1.SelectedIndex == 1)
            {
                ICommand command = new addAll(type);
                command.OnCreate(m_PageLayControl.Object);
                command.OnClick();
                axPageLayoutControl1.CurrentTool = command as ITool;
            }
        }

        private void attributeTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenAttributeForm open = new OpenAttributeForm(m_layerSelected,m_mapControl.Object);
            open.OnClick();
        }

        private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddUser a = new AddUser();
            a.ShowDialog();
        }

        private void deleteUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteUser a = new DeleteUser();
            a.ShowDialog();
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modify a = new Modify();
            a.ShowDialog();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchForm a = new searchForm();
            a.ShowDialog();
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void layoutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void rasterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void personToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Person a = new Person();
            a.ShowDialog();
        }

        private void addZongdiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPolygon a = new addPolygon();
            a.OnCreate(m_mapControl.Object);
            a.OnClick();
        }

        private void exportToImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportImage a = new exportImage(m_PageLayControl);
           a.OnCreate(m_mapControl.Object);
           a.OnClick();

        }

        private void axMapControl2_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
           //IMap oldMap = axMapControl2.Map; //Release original map
            //for (int i = 1; i <= axMapControl2.LayerCount; i++)
            //{
            //    axMapControl1.AddLayer(axMapControl2.get_Layer(axMapControl2.LayerCount - i));
            //}
            ReLoadLayersToHawkEye();
            axMapControl1.Extent = axMapControl1.FullExtent;
        }

        private void chooseAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            chooseAddress a = new chooseAddress(m_mapControl);
            a.OnCreate(m_mapControl.Object);
            a.OnClick();

        }








    
    }
}