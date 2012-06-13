namespace MetaEditor.Forms
{
    partial class ListEntItems
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// <remarks></remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EntItemsTreeView = new System.Windows.Forms.TreeView();
            this.buttGoTo = new System.Windows.Forms.Button();
            this.buttSaveCurrentItem = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.labName = new System.Windows.Forms.Label();
            this.combType = new System.Windows.Forms.ComboBox();
            this.labType = new System.Windows.Forms.Label();
            this.labVisible = new System.Windows.Forms.Label();
            this.radBTrue = new System.Windows.Forms.RadioButton();
            this.radBFalse = new System.Windows.Forms.RadioButton();
            this.txtbOffset = new System.Windows.Forms.TextBox();
            this.labOffset = new System.Windows.Forms.Label();
            this.ButtWritePlugin = new System.Windows.Forms.Button();
            this.panVisible = new System.Windows.Forms.Panel();
            this.labReflexiveChunkSize = new System.Windows.Forms.Label();
            this.txtbReflexiveChunkSize = new System.Windows.Forms.TextBox();
            this.panReflexiveHasCount = new System.Windows.Forms.Panel();
            this.labReflexiveHasCount = new System.Windows.Forms.Label();
            this.radbReflexiveHCTrue = new System.Windows.Forms.RadioButton();
            this.radbReflexiveHCFalse = new System.Windows.Forms.RadioButton();
            this.radbStringString = new System.Windows.Forms.RadioButton();
            this.radbStringUnicode = new System.Windows.Forms.RadioButton();
            this.combStringSize = new System.Windows.Forms.ComboBox();
            this.labStringSize = new System.Windows.Forms.Label();
            this.panStringType = new System.Windows.Forms.Panel();
            this.panStringContainer = new System.Windows.Forms.Panel();
            this.panReflexiveContainer = new System.Windows.Forms.Panel();
            this.combReflexiveLabel = new System.Windows.Forms.ComboBox();
            this.labReflexiveLabel = new System.Windows.Forms.Label();
            this.panIndices = new System.Windows.Forms.Panel();
            this.combIndicesItem = new System.Windows.Forms.ComboBox();
            this.labIndexItemToUseAsLabel = new System.Windows.Forms.Label();
            this.buttIndexCreate = new System.Windows.Forms.Button();
            this.buttIndexDelete = new System.Windows.Forms.Button();
            this.labIndexLayer = new System.Windows.Forms.Label();
            this.combIndicesLayer = new System.Windows.Forms.ComboBox();
            this.combIndicesRToIndex = new System.Windows.Forms.ComboBox();
            this.labIndexReflexive = new System.Windows.Forms.Label();
            this.panBitmask = new System.Windows.Forms.Panel();
            this.buttBitmaskMoveDown = new System.Windows.Forms.Button();
            this.buttBitmaskMoveUp = new System.Windows.Forms.Button();
            this.buttBitmaskCreate = new System.Windows.Forms.Button();
            this.buttBitmaskDelete = new System.Windows.Forms.Button();
            this.buttBitmaskSave = new System.Windows.Forms.Button();
            this.labBitmaskName = new System.Windows.Forms.Label();
            this.labBitmaskBitNumber = new System.Windows.Forms.Label();
            this.txtbBitmaskBitNumber = new System.Windows.Forms.TextBox();
            this.txtbBitmaskName = new System.Windows.Forms.TextBox();
            this.combBitmaskBits = new System.Windows.Forms.ComboBox();
            this.labBitmaskbits = new System.Windows.Forms.Label();
            this.panEnums = new System.Windows.Forms.Panel();
            this.buttEnumsSave = new System.Windows.Forms.Button();
            this.buttEnumsMoveItemDownOne = new System.Windows.Forms.Button();
            this.buttEnumsMoveUpOne = new System.Windows.Forms.Button();
            this.buttEnumsCreate = new System.Windows.Forms.Button();
            this.buttEnumsDelete = new System.Windows.Forms.Button();
            this.txtbEnumsValue = new System.Windows.Forms.TextBox();
            this.txtbEnumsName = new System.Windows.Forms.TextBox();
            this.labEnumsValue = new System.Windows.Forms.Label();
            this.labEnumsName = new System.Windows.Forms.Label();
            this.combEnumsItems = new System.Windows.Forms.ComboBox();
            this.labEnumsItems = new System.Windows.Forms.Label();
            this.buttItemDelete = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.buttAddItemBeforeSelected = new System.Windows.Forms.Button();
            this.buttAddItemAfterSelected = new System.Windows.Forms.Button();
            this.buttAddChildOfSelected = new System.Windows.Forms.Button();
            this.panVisible.SuspendLayout();
            this.panReflexiveHasCount.SuspendLayout();
            this.panStringType.SuspendLayout();
            this.panStringContainer.SuspendLayout();
            this.panReflexiveContainer.SuspendLayout();
            this.panIndices.SuspendLayout();
            this.panBitmask.SuspendLayout();
            this.panEnums.SuspendLayout();
            this.SuspendLayout();
            // 
            // EntItemsTreeView
            // 
            this.EntItemsTreeView.Location = new System.Drawing.Point(12, 12);
            this.EntItemsTreeView.Name = "EntItemsTreeView";
            this.EntItemsTreeView.Size = new System.Drawing.Size(348, 311);
            this.EntItemsTreeView.TabIndex = 0;
            this.EntItemsTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.EntItemsTreeView_NodeMouseDoubleClick);
            this.EntItemsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.EntItemsTreeView_AfterSelect);
            // 
            // buttGoTo
            // 
            this.buttGoTo.Location = new System.Drawing.Point(366, 12);
            this.buttGoTo.Name = "buttGoTo";
            this.buttGoTo.Size = new System.Drawing.Size(140, 35);
            this.buttGoTo.TabIndex = 2;
            this.buttGoTo.Text = "Go to selected item in the meta editor";
            this.buttGoTo.UseVisualStyleBackColor = true;
            this.buttGoTo.Click += new System.EventHandler(this.buttGoTo_Click);
            // 
            // buttSaveCurrentItem
            // 
            this.buttSaveCurrentItem.Location = new System.Drawing.Point(366, 271);
            this.buttSaveCurrentItem.Name = "buttSaveCurrentItem";
            this.buttSaveCurrentItem.Size = new System.Drawing.Size(140, 23);
            this.buttSaveCurrentItem.TabIndex = 3;
            this.buttSaveCurrentItem.Text = " Update Selected item";
            this.buttSaveCurrentItem.UseVisualStyleBackColor = true;
            this.buttSaveCurrentItem.Click += new System.EventHandler(this.buttSaveCurrentItem_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 329);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(456, 20);
            this.txtName.TabIndex = 4;
            // 
            // labName
            // 
            this.labName.AutoSize = true;
            this.labName.Location = new System.Drawing.Point(9, 332);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(35, 13);
            this.labName.TabIndex = 5;
            this.labName.Text = "Name";
            // 
            // combType
            // 
            this.combType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combType.FormattingEnabled = true;
            this.combType.Location = new System.Drawing.Point(49, 355);
            this.combType.Name = "combType";
            this.combType.Size = new System.Drawing.Size(146, 21);
            this.combType.TabIndex = 6;
            this.combType.SelectionChangeCommitted += new System.EventHandler(this.combType_SelectionChangeCommitted);
            // 
            // labType
            // 
            this.labType.AutoSize = true;
            this.labType.Location = new System.Drawing.Point(9, 358);
            this.labType.Name = "labType";
            this.labType.Size = new System.Drawing.Size(31, 13);
            this.labType.TabIndex = 7;
            this.labType.Text = "Type";
            // 
            // labVisible
            // 
            this.labVisible.AutoSize = true;
            this.labVisible.Location = new System.Drawing.Point(0, 3);
            this.labVisible.Name = "labVisible";
            this.labVisible.Size = new System.Drawing.Size(37, 13);
            this.labVisible.TabIndex = 8;
            this.labVisible.Text = "Visible";
            // 
            // radBTrue
            // 
            this.radBTrue.AutoSize = true;
            this.radBTrue.Location = new System.Drawing.Point(43, 1);
            this.radBTrue.Name = "radBTrue";
            this.radBTrue.Size = new System.Drawing.Size(47, 17);
            this.radBTrue.TabIndex = 9;
            this.radBTrue.TabStop = true;
            this.radBTrue.Text = "True";
            this.radBTrue.UseVisualStyleBackColor = true;
            // 
            // radBFalse
            // 
            this.radBFalse.AutoSize = true;
            this.radBFalse.Location = new System.Drawing.Point(96, 1);
            this.radBFalse.Name = "radBFalse";
            this.radBFalse.Size = new System.Drawing.Size(50, 17);
            this.radBFalse.TabIndex = 10;
            this.radBFalse.TabStop = true;
            this.radBFalse.Text = "False";
            this.radBFalse.UseVisualStyleBackColor = true;
            // 
            // txtbOffset
            // 
            this.txtbOffset.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtbOffset.Location = new System.Drawing.Point(395, 355);
            this.txtbOffset.Name = "txtbOffset";
            this.txtbOffset.ReadOnly = true;
            this.txtbOffset.Size = new System.Drawing.Size(111, 20);
            this.txtbOffset.TabIndex = 11;
            // 
            // labOffset
            // 
            this.labOffset.AutoSize = true;
            this.labOffset.Location = new System.Drawing.Point(354, 358);
            this.labOffset.Name = "labOffset";
            this.labOffset.Size = new System.Drawing.Size(35, 13);
            this.labOffset.TabIndex = 12;
            this.labOffset.Text = "Offset";
            // 
            // ButtWritePlugin
            // 
            this.ButtWritePlugin.Location = new System.Drawing.Point(366, 300);
            this.ButtWritePlugin.Name = "ButtWritePlugin";
            this.ButtWritePlugin.Size = new System.Drawing.Size(140, 23);
            this.ButtWritePlugin.TabIndex = 13;
            this.ButtWritePlugin.Text = "Save To Plugin";
            this.ButtWritePlugin.UseVisualStyleBackColor = true;
            this.ButtWritePlugin.Click += new System.EventHandler(this.ButtWritePlugin_Click);
            // 
            // panVisible
            // 
            this.panVisible.Controls.Add(this.labVisible);
            this.panVisible.Controls.Add(this.radBFalse);
            this.panVisible.Controls.Add(this.radBTrue);
            this.panVisible.Location = new System.Drawing.Point(201, 355);
            this.panVisible.Name = "panVisible";
            this.panVisible.Size = new System.Drawing.Size(146, 21);
            this.panVisible.TabIndex = 14;
            // 
            // labReflexiveChunkSize
            // 
            this.labReflexiveChunkSize.AutoSize = true;
            this.labReflexiveChunkSize.Location = new System.Drawing.Point(-3, 3);
            this.labReflexiveChunkSize.Name = "labReflexiveChunkSize";
            this.labReflexiveChunkSize.Size = new System.Drawing.Size(61, 13);
            this.labReflexiveChunkSize.TabIndex = 5;
            this.labReflexiveChunkSize.Text = "Chunk Size";
            // 
            // txtbReflexiveChunkSize
            // 
            this.txtbReflexiveChunkSize.Location = new System.Drawing.Point(64, 0);
            this.txtbReflexiveChunkSize.Name = "txtbReflexiveChunkSize";
            this.txtbReflexiveChunkSize.Size = new System.Drawing.Size(99, 20);
            this.txtbReflexiveChunkSize.TabIndex = 4;
            // 
            // panReflexiveHasCount
            // 
            this.panReflexiveHasCount.Controls.Add(this.labReflexiveHasCount);
            this.panReflexiveHasCount.Controls.Add(this.radbReflexiveHCTrue);
            this.panReflexiveHasCount.Controls.Add(this.radbReflexiveHCFalse);
            this.panReflexiveHasCount.Location = new System.Drawing.Point(169, 3);
            this.panReflexiveHasCount.Name = "panReflexiveHasCount";
            this.panReflexiveHasCount.Size = new System.Drawing.Size(165, 17);
            this.panReflexiveHasCount.TabIndex = 3;
            // 
            // labReflexiveHasCount
            // 
            this.labReflexiveHasCount.AutoSize = true;
            this.labReflexiveHasCount.Location = new System.Drawing.Point(-1, 3);
            this.labReflexiveHasCount.Name = "labReflexiveHasCount";
            this.labReflexiveHasCount.Size = new System.Drawing.Size(57, 13);
            this.labReflexiveHasCount.TabIndex = 0;
            this.labReflexiveHasCount.Text = "Has Count";
            // 
            // radbReflexiveHCTrue
            // 
            this.radbReflexiveHCTrue.AutoSize = true;
            this.radbReflexiveHCTrue.Location = new System.Drawing.Point(62, 1);
            this.radbReflexiveHCTrue.Name = "radbReflexiveHCTrue";
            this.radbReflexiveHCTrue.Size = new System.Drawing.Size(47, 17);
            this.radbReflexiveHCTrue.TabIndex = 9;
            this.radbReflexiveHCTrue.TabStop = true;
            this.radbReflexiveHCTrue.Text = "True";
            this.radbReflexiveHCTrue.UseVisualStyleBackColor = true;
            // 
            // radbReflexiveHCFalse
            // 
            this.radbReflexiveHCFalse.AutoSize = true;
            this.radbReflexiveHCFalse.Location = new System.Drawing.Point(115, 1);
            this.radbReflexiveHCFalse.Name = "radbReflexiveHCFalse";
            this.radbReflexiveHCFalse.Size = new System.Drawing.Size(50, 17);
            this.radbReflexiveHCFalse.TabIndex = 10;
            this.radbReflexiveHCFalse.TabStop = true;
            this.radbReflexiveHCFalse.Text = "False";
            this.radbReflexiveHCFalse.UseVisualStyleBackColor = true;
            // 
            // radbStringString
            // 
            this.radbStringString.AutoSize = true;
            this.radbStringString.Location = new System.Drawing.Point(3, 1);
            this.radbStringString.Name = "radbStringString";
            this.radbStringString.Size = new System.Drawing.Size(52, 17);
            this.radbStringString.TabIndex = 0;
            this.radbStringString.TabStop = true;
            this.radbStringString.Text = "String";
            this.radbStringString.UseVisualStyleBackColor = true;
            // 
            // radbStringUnicode
            // 
            this.radbStringUnicode.AutoSize = true;
            this.radbStringUnicode.Location = new System.Drawing.Point(61, 1);
            this.radbStringUnicode.Name = "radbStringUnicode";
            this.radbStringUnicode.Size = new System.Drawing.Size(65, 17);
            this.radbStringUnicode.TabIndex = 1;
            this.radbStringUnicode.TabStop = true;
            this.radbStringUnicode.Text = "Unicode";
            this.radbStringUnicode.UseVisualStyleBackColor = true;
            // 
            // combStringSize
            // 
            this.combStringSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combStringSize.FormattingEnabled = true;
            this.combStringSize.Items.AddRange(new object[] {
            "32",
            "64",
            "256"});
            this.combStringSize.Location = new System.Drawing.Point(30, 0);
            this.combStringSize.Name = "combStringSize";
            this.combStringSize.Size = new System.Drawing.Size(57, 21);
            this.combStringSize.TabIndex = 15;
            // 
            // labStringSize
            // 
            this.labStringSize.AutoSize = true;
            this.labStringSize.Location = new System.Drawing.Point(-3, 3);
            this.labStringSize.Name = "labStringSize";
            this.labStringSize.Size = new System.Drawing.Size(27, 13);
            this.labStringSize.TabIndex = 16;
            this.labStringSize.Text = "Size";
            // 
            // panStringType
            // 
            this.panStringType.Controls.Add(this.radbStringUnicode);
            this.panStringType.Controls.Add(this.radbStringString);
            this.panStringType.Location = new System.Drawing.Point(93, 0);
            this.panStringType.Name = "panStringType";
            this.panStringType.Size = new System.Drawing.Size(128, 21);
            this.panStringType.TabIndex = 18;
            // 
            // panStringContainer
            // 
            this.panStringContainer.Controls.Add(this.labStringSize);
            this.panStringContainer.Controls.Add(this.combStringSize);
            this.panStringContainer.Controls.Add(this.panStringType);
            this.panStringContainer.Location = new System.Drawing.Point(12, 382);
            this.panStringContainer.Name = "panStringContainer";
            this.panStringContainer.Size = new System.Drawing.Size(225, 25);
            this.panStringContainer.TabIndex = 19;
            // 
            // panReflexiveContainer
            // 
            this.panReflexiveContainer.Controls.Add(this.combReflexiveLabel);
            this.panReflexiveContainer.Controls.Add(this.labReflexiveLabel);
            this.panReflexiveContainer.Controls.Add(this.panReflexiveHasCount);
            this.panReflexiveContainer.Controls.Add(this.labReflexiveChunkSize);
            this.panReflexiveContainer.Controls.Add(this.txtbReflexiveChunkSize);
            this.panReflexiveContainer.Location = new System.Drawing.Point(12, 382);
            this.panReflexiveContainer.Name = "panReflexiveContainer";
            this.panReflexiveContainer.Size = new System.Drawing.Size(494, 65);
            this.panReflexiveContainer.TabIndex = 20;
            // 
            // combReflexiveLabel
            // 
            this.combReflexiveLabel.FormattingEnabled = true;
            this.combReflexiveLabel.Location = new System.Drawing.Point(38, 26);
            this.combReflexiveLabel.Name = "combReflexiveLabel";
            this.combReflexiveLabel.Size = new System.Drawing.Size(209, 21);
            this.combReflexiveLabel.TabIndex = 7;
            // 
            // labReflexiveLabel
            // 
            this.labReflexiveLabel.AutoSize = true;
            this.labReflexiveLabel.Location = new System.Drawing.Point(-1, 29);
            this.labReflexiveLabel.Name = "labReflexiveLabel";
            this.labReflexiveLabel.Size = new System.Drawing.Size(33, 13);
            this.labReflexiveLabel.TabIndex = 6;
            this.labReflexiveLabel.Text = "Label";
            // 
            // panIndices
            // 
            this.panIndices.Controls.Add(this.combIndicesItem);
            this.panIndices.Controls.Add(this.labIndexItemToUseAsLabel);
            this.panIndices.Controls.Add(this.buttIndexCreate);
            this.panIndices.Controls.Add(this.buttIndexDelete);
            this.panIndices.Controls.Add(this.labIndexLayer);
            this.panIndices.Controls.Add(this.combIndicesLayer);
            this.panIndices.Controls.Add(this.combIndicesRToIndex);
            this.panIndices.Controls.Add(this.labIndexReflexive);
            this.panIndices.Location = new System.Drawing.Point(12, 382);
            this.panIndices.Name = "panIndices";
            this.panIndices.Size = new System.Drawing.Size(494, 65);
            this.panIndices.TabIndex = 20;
            // 
            // combIndicesItem
            // 
            this.combIndicesItem.FormattingEnabled = true;
            this.combIndicesItem.Location = new System.Drawing.Point(75, 27);
            this.combIndicesItem.Name = "combIndicesItem";
            this.combIndicesItem.Size = new System.Drawing.Size(182, 21);
            this.combIndicesItem.TabIndex = 15;
            // 
            // labIndexItemToUseAsLabel
            // 
            this.labIndexItemToUseAsLabel.AutoSize = true;
            this.labIndexItemToUseAsLabel.Location = new System.Drawing.Point(-3, 30);
            this.labIndexItemToUseAsLabel.Name = "labIndexItemToUseAsLabel";
            this.labIndexItemToUseAsLabel.Size = new System.Drawing.Size(72, 13);
            this.labIndexItemToUseAsLabel.TabIndex = 14;
            this.labIndexItemToUseAsLabel.Text = "Item To Index";
            // 
            // buttIndexCreate
            // 
            this.buttIndexCreate.Location = new System.Drawing.Point(373, 25);
            this.buttIndexCreate.Name = "buttIndexCreate";
            this.buttIndexCreate.Size = new System.Drawing.Size(121, 23);
            this.buttIndexCreate.TabIndex = 13;
            this.buttIndexCreate.Text = "Create Index";
            this.buttIndexCreate.UseVisualStyleBackColor = true;
            this.buttIndexCreate.Click += new System.EventHandler(this.buttIndexCreate_Click);
            // 
            // buttIndexDelete
            // 
            this.buttIndexDelete.Location = new System.Drawing.Point(373, 25);
            this.buttIndexDelete.Name = "buttIndexDelete";
            this.buttIndexDelete.Size = new System.Drawing.Size(121, 23);
            this.buttIndexDelete.TabIndex = 12;
            this.buttIndexDelete.Text = "Delete Index";
            this.buttIndexDelete.UseVisualStyleBackColor = true;
            this.buttIndexDelete.Click += new System.EventHandler(this.buttIndexDelete_Click);
            // 
            // labIndexLayer
            // 
            this.labIndexLayer.AutoSize = true;
            this.labIndexLayer.Location = new System.Drawing.Point(287, 3);
            this.labIndexLayer.Name = "labIndexLayer";
            this.labIndexLayer.Size = new System.Drawing.Size(80, 13);
            this.labIndexLayer.TabIndex = 11;
            this.labIndexLayer.Text = "Reflexive Layer";
            // 
            // combIndicesLayer
            // 
            this.combIndicesLayer.FormattingEnabled = true;
            this.combIndicesLayer.Items.AddRange(new object[] {
            "",
            "root",
            "oneup"});
            this.combIndicesLayer.Location = new System.Drawing.Point(373, 0);
            this.combIndicesLayer.Name = "combIndicesLayer";
            this.combIndicesLayer.Size = new System.Drawing.Size(121, 21);
            this.combIndicesLayer.TabIndex = 10;
            this.combIndicesLayer.DropDownClosed += new System.EventHandler(this.combIndicesLayer_DropDownClosed);
            // 
            // combIndicesRToIndex
            // 
            this.combIndicesRToIndex.FormattingEnabled = true;
            this.combIndicesRToIndex.Location = new System.Drawing.Point(99, 0);
            this.combIndicesRToIndex.Name = "combIndicesRToIndex";
            this.combIndicesRToIndex.Size = new System.Drawing.Size(182, 21);
            this.combIndicesRToIndex.TabIndex = 9;
            this.combIndicesRToIndex.DropDownClosed += new System.EventHandler(this.combIndicesRToIndex_DropDownClosed);
            // 
            // labIndexReflexive
            // 
            this.labIndexReflexive.AutoSize = true;
            this.labIndexReflexive.Location = new System.Drawing.Point(-3, 3);
            this.labIndexReflexive.Name = "labIndexReflexive";
            this.labIndexReflexive.Size = new System.Drawing.Size(96, 13);
            this.labIndexReflexive.TabIndex = 8;
            this.labIndexReflexive.Text = "Reflexive To Index";
            // 
            // panBitmask
            // 
            this.panBitmask.Controls.Add(this.buttBitmaskMoveDown);
            this.panBitmask.Controls.Add(this.buttBitmaskMoveUp);
            this.panBitmask.Controls.Add(this.buttBitmaskCreate);
            this.panBitmask.Controls.Add(this.buttBitmaskDelete);
            this.panBitmask.Controls.Add(this.buttBitmaskSave);
            this.panBitmask.Controls.Add(this.labBitmaskName);
            this.panBitmask.Controls.Add(this.labBitmaskBitNumber);
            this.panBitmask.Controls.Add(this.txtbBitmaskBitNumber);
            this.panBitmask.Controls.Add(this.txtbBitmaskName);
            this.panBitmask.Controls.Add(this.combBitmaskBits);
            this.panBitmask.Controls.Add(this.labBitmaskbits);
            this.panBitmask.Location = new System.Drawing.Point(12, 382);
            this.panBitmask.Name = "panBitmask";
            this.panBitmask.Size = new System.Drawing.Size(494, 60);
            this.panBitmask.TabIndex = 21;
            // 
            // buttBitmaskMoveDown
            // 
            this.buttBitmaskMoveDown.Location = new System.Drawing.Point(285, 26);
            this.buttBitmaskMoveDown.Name = "buttBitmaskMoveDown";
            this.buttBitmaskMoveDown.Size = new System.Drawing.Size(128, 23);
            this.buttBitmaskMoveDown.TabIndex = 10;
            this.buttBitmaskMoveDown.Text = "Move Down One Slot";
            this.buttBitmaskMoveDown.UseVisualStyleBackColor = true;
            this.buttBitmaskMoveDown.Click += new System.EventHandler(this.buttBitmaskMoveDown_Click);
            // 
            // buttBitmaskMoveUp
            // 
            this.buttBitmaskMoveUp.Location = new System.Drawing.Point(162, 26);
            this.buttBitmaskMoveUp.Name = "buttBitmaskMoveUp";
            this.buttBitmaskMoveUp.Size = new System.Drawing.Size(117, 23);
            this.buttBitmaskMoveUp.TabIndex = 9;
            this.buttBitmaskMoveUp.Text = "Move Up One Slot";
            this.buttBitmaskMoveUp.UseVisualStyleBackColor = true;
            this.buttBitmaskMoveUp.Click += new System.EventHandler(this.buttBitmaskMoveUp_Click);
            // 
            // buttBitmaskCreate
            // 
            this.buttBitmaskCreate.Location = new System.Drawing.Point(81, 26);
            this.buttBitmaskCreate.Name = "buttBitmaskCreate";
            this.buttBitmaskCreate.Size = new System.Drawing.Size(75, 23);
            this.buttBitmaskCreate.TabIndex = 8;
            this.buttBitmaskCreate.Text = "Create";
            this.buttBitmaskCreate.UseVisualStyleBackColor = true;
            this.buttBitmaskCreate.Click += new System.EventHandler(this.buttBitmaskCreate_Click);
            // 
            // buttBitmaskDelete
            // 
            this.buttBitmaskDelete.Location = new System.Drawing.Point(0, 26);
            this.buttBitmaskDelete.Name = "buttBitmaskDelete";
            this.buttBitmaskDelete.Size = new System.Drawing.Size(75, 23);
            this.buttBitmaskDelete.TabIndex = 7;
            this.buttBitmaskDelete.Text = "Delete";
            this.buttBitmaskDelete.UseVisualStyleBackColor = true;
            this.buttBitmaskDelete.Click += new System.EventHandler(this.buttBitmaskDelete_Click);
            // 
            // buttBitmaskSave
            // 
            this.buttBitmaskSave.Location = new System.Drawing.Point(419, 26);
            this.buttBitmaskSave.Name = "buttBitmaskSave";
            this.buttBitmaskSave.Size = new System.Drawing.Size(75, 23);
            this.buttBitmaskSave.TabIndex = 6;
            this.buttBitmaskSave.Text = "Save";
            this.buttBitmaskSave.UseVisualStyleBackColor = true;
            this.buttBitmaskSave.Click += new System.EventHandler(this.buttBitmaskSave_Click);
            // 
            // labBitmaskName
            // 
            this.labBitmaskName.AutoSize = true;
            this.labBitmaskName.Location = new System.Drawing.Point(189, 3);
            this.labBitmaskName.Name = "labBitmaskName";
            this.labBitmaskName.Size = new System.Drawing.Size(35, 13);
            this.labBitmaskName.TabIndex = 5;
            this.labBitmaskName.Text = "Name";
            // 
            // labBitmaskBitNumber
            // 
            this.labBitmaskBitNumber.AutoSize = true;
            this.labBitmaskBitNumber.Location = new System.Drawing.Point(354, 3);
            this.labBitmaskBitNumber.Name = "labBitmaskBitNumber";
            this.labBitmaskBitNumber.Size = new System.Drawing.Size(19, 13);
            this.labBitmaskBitNumber.TabIndex = 4;
            this.labBitmaskBitNumber.Text = "Bit";
            // 
            // txtbBitmaskBitNumber
            // 
            this.txtbBitmaskBitNumber.Location = new System.Drawing.Point(379, 0);
            this.txtbBitmaskBitNumber.Name = "txtbBitmaskBitNumber";
            this.txtbBitmaskBitNumber.Size = new System.Drawing.Size(115, 20);
            this.txtbBitmaskBitNumber.TabIndex = 3;
            // 
            // txtbBitmaskName
            // 
            this.txtbBitmaskName.Location = new System.Drawing.Point(230, 0);
            this.txtbBitmaskName.Name = "txtbBitmaskName";
            this.txtbBitmaskName.Size = new System.Drawing.Size(118, 20);
            this.txtbBitmaskName.TabIndex = 2;
            // 
            // combBitmaskBits
            // 
            this.combBitmaskBits.FormattingEnabled = true;
            this.combBitmaskBits.Location = new System.Drawing.Point(27, 0);
            this.combBitmaskBits.Name = "combBitmaskBits";
            this.combBitmaskBits.Size = new System.Drawing.Size(156, 21);
            this.combBitmaskBits.TabIndex = 1;
            this.combBitmaskBits.DropDownClosed += new System.EventHandler(this.combBitmaskBits_DropDownClosed);
            // 
            // labBitmaskbits
            // 
            this.labBitmaskbits.AutoSize = true;
            this.labBitmaskbits.Location = new System.Drawing.Point(-3, 3);
            this.labBitmaskbits.Name = "labBitmaskbits";
            this.labBitmaskbits.Size = new System.Drawing.Size(24, 13);
            this.labBitmaskbits.TabIndex = 0;
            this.labBitmaskbits.Text = "Bits";
            // 
            // panEnums
            // 
            this.panEnums.Controls.Add(this.buttEnumsSave);
            this.panEnums.Controls.Add(this.buttEnumsMoveItemDownOne);
            this.panEnums.Controls.Add(this.buttEnumsMoveUpOne);
            this.panEnums.Controls.Add(this.buttEnumsCreate);
            this.panEnums.Controls.Add(this.buttEnumsDelete);
            this.panEnums.Controls.Add(this.txtbEnumsValue);
            this.panEnums.Controls.Add(this.txtbEnumsName);
            this.panEnums.Controls.Add(this.labEnumsValue);
            this.panEnums.Controls.Add(this.labEnumsName);
            this.panEnums.Controls.Add(this.combEnumsItems);
            this.panEnums.Controls.Add(this.labEnumsItems);
            this.panEnums.Location = new System.Drawing.Point(12, 382);
            this.panEnums.Name = "panEnums";
            this.panEnums.Size = new System.Drawing.Size(494, 57);
            this.panEnums.TabIndex = 23;
            // 
            // buttEnumsSave
            // 
            this.buttEnumsSave.Location = new System.Drawing.Point(419, 26);
            this.buttEnumsSave.Name = "buttEnumsSave";
            this.buttEnumsSave.Size = new System.Drawing.Size(75, 23);
            this.buttEnumsSave.TabIndex = 11;
            this.buttEnumsSave.Text = "Save";
            this.buttEnumsSave.UseVisualStyleBackColor = true;
            this.buttEnumsSave.Click += new System.EventHandler(this.buttEnumsSave_Click);
            // 
            // buttEnumsMoveItemDownOne
            // 
            this.buttEnumsMoveItemDownOne.Location = new System.Drawing.Point(285, 27);
            this.buttEnumsMoveItemDownOne.Name = "buttEnumsMoveItemDownOne";
            this.buttEnumsMoveItemDownOne.Size = new System.Drawing.Size(128, 23);
            this.buttEnumsMoveItemDownOne.TabIndex = 10;
            this.buttEnumsMoveItemDownOne.Text = "Move Item Down One";
            this.buttEnumsMoveItemDownOne.UseVisualStyleBackColor = true;
            this.buttEnumsMoveItemDownOne.Click += new System.EventHandler(this.buttEnumsMoveItemDownOne_Click);
            // 
            // buttEnumsMoveUpOne
            // 
            this.buttEnumsMoveUpOne.Location = new System.Drawing.Point(165, 27);
            this.buttEnumsMoveUpOne.Name = "buttEnumsMoveUpOne";
            this.buttEnumsMoveUpOne.Size = new System.Drawing.Size(114, 23);
            this.buttEnumsMoveUpOne.TabIndex = 9;
            this.buttEnumsMoveUpOne.Text = "Move Item Up One";
            this.buttEnumsMoveUpOne.UseVisualStyleBackColor = true;
            this.buttEnumsMoveUpOne.Click += new System.EventHandler(this.buttEnumsMoveUpOne_Click);
            // 
            // buttEnumsCreate
            // 
            this.buttEnumsCreate.Location = new System.Drawing.Point(84, 27);
            this.buttEnumsCreate.Name = "buttEnumsCreate";
            this.buttEnumsCreate.Size = new System.Drawing.Size(75, 23);
            this.buttEnumsCreate.TabIndex = 7;
            this.buttEnumsCreate.Text = "Create";
            this.buttEnumsCreate.UseVisualStyleBackColor = true;
            this.buttEnumsCreate.Click += new System.EventHandler(this.buttEnumsCreate_Click);
            // 
            // buttEnumsDelete
            // 
            this.buttEnumsDelete.Location = new System.Drawing.Point(3, 27);
            this.buttEnumsDelete.Name = "buttEnumsDelete";
            this.buttEnumsDelete.Size = new System.Drawing.Size(75, 23);
            this.buttEnumsDelete.TabIndex = 6;
            this.buttEnumsDelete.Text = "Delete";
            this.buttEnumsDelete.UseVisualStyleBackColor = true;
            this.buttEnumsDelete.Click += new System.EventHandler(this.buttEnumsDelete_Click);
            // 
            // txtbEnumsValue
            // 
            this.txtbEnumsValue.Location = new System.Drawing.Point(403, 0);
            this.txtbEnumsValue.Name = "txtbEnumsValue";
            this.txtbEnumsValue.Size = new System.Drawing.Size(91, 20);
            this.txtbEnumsValue.TabIndex = 5;
            // 
            // txtbEnumsName
            // 
            this.txtbEnumsName.Location = new System.Drawing.Point(230, 0);
            this.txtbEnumsName.Name = "txtbEnumsName";
            this.txtbEnumsName.Size = new System.Drawing.Size(127, 20);
            this.txtbEnumsName.TabIndex = 4;
            // 
            // labEnumsValue
            // 
            this.labEnumsValue.AutoSize = true;
            this.labEnumsValue.Location = new System.Drawing.Point(363, 3);
            this.labEnumsValue.Name = "labEnumsValue";
            this.labEnumsValue.Size = new System.Drawing.Size(34, 13);
            this.labEnumsValue.TabIndex = 3;
            this.labEnumsValue.Text = "Value";
            // 
            // labEnumsName
            // 
            this.labEnumsName.AutoSize = true;
            this.labEnumsName.Location = new System.Drawing.Point(189, 3);
            this.labEnumsName.Name = "labEnumsName";
            this.labEnumsName.Size = new System.Drawing.Size(35, 13);
            this.labEnumsName.TabIndex = 2;
            this.labEnumsName.Text = "Name";
            // 
            // combEnumsItems
            // 
            this.combEnumsItems.FormattingEnabled = true;
            this.combEnumsItems.Location = new System.Drawing.Point(35, 0);
            this.combEnumsItems.Name = "combEnumsItems";
            this.combEnumsItems.Size = new System.Drawing.Size(148, 21);
            this.combEnumsItems.TabIndex = 1;
            this.combEnumsItems.DropDownClosed += new System.EventHandler(this.combEnumsItems_DropDownClosed);
            // 
            // labEnumsItems
            // 
            this.labEnumsItems.AutoSize = true;
            this.labEnumsItems.Location = new System.Drawing.Point(-3, 3);
            this.labEnumsItems.Name = "labEnumsItems";
            this.labEnumsItems.Size = new System.Drawing.Size(32, 13);
            this.labEnumsItems.TabIndex = 0;
            this.labEnumsItems.Text = "Items";
            // 
            // buttItemDelete
            // 
            this.buttItemDelete.Location = new System.Drawing.Point(366, 213);
            this.buttItemDelete.Name = "buttItemDelete";
            this.buttItemDelete.Size = new System.Drawing.Size(140, 23);
            this.buttItemDelete.TabIndex = 24;
            this.buttItemDelete.Text = "Delete Item";
            this.buttItemDelete.UseVisualStyleBackColor = true;
            this.buttItemDelete.Click += new System.EventHandler(this.buttItemDelete_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Reflexive",
            "StringID",
            "TagType",
            "Ident",
            "String32",
            "UnicodeString64",
            "String256",
            "UnicodeString256",
            "Unused",
            "Unknown",
            "Float",
            "Int",
            "UInt",
            "Short",
            "UShort",
            "Byte",
            "Bitmask32",
            "Bitmask16",
            "Bitmask8",
            "Enum32",
            "Enum16",
            "Enum8",
            "Option"});
            this.comboBox1.Location = new System.Drawing.Point(366, 99);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(140, 21);
            this.comboBox1.TabIndex = 26;
            // 
            // buttAddItemBeforeSelected
            // 
            this.buttAddItemBeforeSelected.Location = new System.Drawing.Point(366, 126);
            this.buttAddItemBeforeSelected.Name = "buttAddItemBeforeSelected";
            this.buttAddItemBeforeSelected.Size = new System.Drawing.Size(140, 23);
            this.buttAddItemBeforeSelected.TabIndex = 27;
            this.buttAddItemBeforeSelected.Text = "Add Before Selected";
            this.buttAddItemBeforeSelected.UseVisualStyleBackColor = true;
            this.buttAddItemBeforeSelected.Click += new System.EventHandler(this.buttAddItemBeforeSelected_Click);
            // 
            // buttAddItemAfterSelected
            // 
            this.buttAddItemAfterSelected.Location = new System.Drawing.Point(366, 155);
            this.buttAddItemAfterSelected.Name = "buttAddItemAfterSelected";
            this.buttAddItemAfterSelected.Size = new System.Drawing.Size(140, 23);
            this.buttAddItemAfterSelected.TabIndex = 28;
            this.buttAddItemAfterSelected.Text = "Add After Selected";
            this.buttAddItemAfterSelected.UseVisualStyleBackColor = true;
            this.buttAddItemAfterSelected.Visible = false;
            this.buttAddItemAfterSelected.Click += new System.EventHandler(this.buttAddItemAfterSelected_Click);
            // 
            // buttAddChildOfSelected
            // 
            this.buttAddChildOfSelected.Location = new System.Drawing.Point(366, 184);
            this.buttAddChildOfSelected.Name = "buttAddChildOfSelected";
            this.buttAddChildOfSelected.Size = new System.Drawing.Size(140, 23);
            this.buttAddChildOfSelected.TabIndex = 29;
            this.buttAddChildOfSelected.Text = "Add As Child Of Selected";
            this.buttAddChildOfSelected.UseVisualStyleBackColor = true;
            this.buttAddChildOfSelected.Visible = false;
            this.buttAddChildOfSelected.Click += new System.EventHandler(this.buttAddChildOfSelected_Click);
            // 
            // ListEntItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 451);
            this.Controls.Add(this.buttAddChildOfSelected);
            this.Controls.Add(this.buttAddItemAfterSelected);
            this.Controls.Add(this.buttAddItemBeforeSelected);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.buttItemDelete);
            this.Controls.Add(this.buttGoTo);
            this.Controls.Add(this.panStringContainer);
            this.Controls.Add(this.panVisible);
            this.Controls.Add(this.ButtWritePlugin);
            this.Controls.Add(this.labOffset);
            this.Controls.Add(this.txtbOffset);
            this.Controls.Add(this.labType);
            this.Controls.Add(this.combType);
            this.Controls.Add(this.labName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.buttSaveCurrentItem);
            this.Controls.Add(this.EntItemsTreeView);
            this.Controls.Add(this.panEnums);
            this.Controls.Add(this.panBitmask);
            this.Controls.Add(this.panReflexiveContainer);
            this.Controls.Add(this.panIndices);
            this.Name = "ListEntItems";
            this.Text = "Ent Plugin Editor";
            this.panVisible.ResumeLayout(false);
            this.panVisible.PerformLayout();
            this.panReflexiveHasCount.ResumeLayout(false);
            this.panReflexiveHasCount.PerformLayout();
            this.panStringType.ResumeLayout(false);
            this.panStringType.PerformLayout();
            this.panStringContainer.ResumeLayout(false);
            this.panStringContainer.PerformLayout();
            this.panReflexiveContainer.ResumeLayout(false);
            this.panReflexiveContainer.PerformLayout();
            this.panIndices.ResumeLayout(false);
            this.panIndices.PerformLayout();
            this.panBitmask.ResumeLayout(false);
            this.panBitmask.PerformLayout();
            this.panEnums.ResumeLayout(false);
            this.panEnums.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.TreeView EntItemsTreeView;
        private System.Windows.Forms.Button buttGoTo;
        private System.Windows.Forms.Button buttSaveCurrentItem;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label labName;
        private System.Windows.Forms.ComboBox combType;
        private System.Windows.Forms.Label labType;
        private System.Windows.Forms.Label labVisible;
        private System.Windows.Forms.RadioButton radBTrue;
        private System.Windows.Forms.RadioButton radBFalse;
        private System.Windows.Forms.TextBox txtbOffset;
        private System.Windows.Forms.Label labOffset;
        private System.Windows.Forms.Button ButtWritePlugin;
        private System.Windows.Forms.Panel panVisible;
        private System.Windows.Forms.Label labReflexiveChunkSize;
        private System.Windows.Forms.TextBox txtbReflexiveChunkSize;
        private System.Windows.Forms.Panel panReflexiveHasCount;
        private System.Windows.Forms.Label labReflexiveHasCount;
        private System.Windows.Forms.RadioButton radbReflexiveHCFalse;
        private System.Windows.Forms.RadioButton radbReflexiveHCTrue;
        private System.Windows.Forms.ComboBox combStringSize;
        private System.Windows.Forms.Label labStringSize;
        private System.Windows.Forms.Panel panStringType;
        private System.Windows.Forms.RadioButton radbStringUnicode;
        private System.Windows.Forms.RadioButton radbStringString;
        private System.Windows.Forms.Panel panStringContainer;
        private System.Windows.Forms.Panel panReflexiveContainer;
        private System.Windows.Forms.Panel panIndices;
        private System.Windows.Forms.ComboBox combIndicesRToIndex;
        private System.Windows.Forms.Label labIndexReflexive;
        private System.Windows.Forms.ComboBox combIndicesLayer;
        private System.Windows.Forms.Label labIndexLayer;
        private System.Windows.Forms.Button buttIndexCreate;
        private System.Windows.Forms.Button buttIndexDelete;
        private System.Windows.Forms.Label labIndexItemToUseAsLabel;
        private System.Windows.Forms.ComboBox combIndicesItem;
        private System.Windows.Forms.ComboBox combReflexiveLabel;
        private System.Windows.Forms.Label labReflexiveLabel;
        private System.Windows.Forms.Panel panBitmask;
        private System.Windows.Forms.Label labBitmaskbits;
        private System.Windows.Forms.ComboBox combBitmaskBits;
        private System.Windows.Forms.Label labBitmaskName;
        private System.Windows.Forms.Label labBitmaskBitNumber;
        private System.Windows.Forms.TextBox txtbBitmaskBitNumber;
        private System.Windows.Forms.TextBox txtbBitmaskName;
        private System.Windows.Forms.Button buttBitmaskCreate;
        private System.Windows.Forms.Button buttBitmaskDelete;
        private System.Windows.Forms.Button buttBitmaskSave;
        private System.Windows.Forms.Button buttBitmaskMoveDown;
        private System.Windows.Forms.Button buttBitmaskMoveUp;
        private System.Windows.Forms.Panel panEnums;
        private System.Windows.Forms.Label labEnumsItems;
        private System.Windows.Forms.TextBox txtbEnumsValue;
        private System.Windows.Forms.TextBox txtbEnumsName;
        private System.Windows.Forms.Label labEnumsValue;
        private System.Windows.Forms.Label labEnumsName;
        private System.Windows.Forms.ComboBox combEnumsItems;
        private System.Windows.Forms.Button buttEnumsMoveItemDownOne;
        private System.Windows.Forms.Button buttEnumsMoveUpOne;
        private System.Windows.Forms.Button buttEnumsCreate;
        private System.Windows.Forms.Button buttEnumsDelete;
        private System.Windows.Forms.Button buttEnumsSave;
        private System.Windows.Forms.Button buttItemDelete;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button buttAddItemBeforeSelected;
        private System.Windows.Forms.Button buttAddItemAfterSelected;
        private System.Windows.Forms.Button buttAddChildOfSelected;
    }
}