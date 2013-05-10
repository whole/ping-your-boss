// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
#if interactive
#r "System.DirectoryServices.dll"
#endif
open System
open System.Collections
open System.Net.NetworkInformation
open System.Threading.Tasks
open System.Windows.Forms
open System.Drawing

let path = "../hosts.txt"
let readFromFile() = 
    System.IO.File.ReadAllLines(path);;
let populateComboBox(savedHosts: string[]) =
    let cmb = new ComboBox(Size = new Size(100, 16), Dock=DockStyle.Top)     
    for i in savedHosts do
        cmb.Items.Add(i) |> ignore
    cmb

let refreshComboBox(cmb : ComboBox) =
    for i in readFromFile() do
        if (cmb.Items.Contains(i) = false) then
            cmb.Items.Add(i) |> ignore
        cmb.Refresh()
    
[<STAThread>]
do Application.EnableVisualStyles()

let cmb = populateComboBox(readFromFile())
let btnSearch = new Button(Text = "Check your boss", Dock=DockStyle.Top)
let checkAllHosts = new Button(Text = "Fetch all computers", Dock=DockStyle.Top)

let alertLabel = new ToolStripLabel(Dock=DockStyle.Right)
alertLabel.BackColor =System.Drawing.Color.Red |> ignore
alertLabel.ForeColor =System.Drawing.Color.Red |> ignore
let toolbarControls = new ToolStrip(Dock=DockStyle.Top)

let address = new ToolStripTextBox(Size=new Size(400, 25))
address.Text <- "enter particular address"
let label = new Label(Dock=DockStyle.Fill)
let toolbar = new ToolStrip(Dock=DockStyle.Top)

let go = new ToolStripButton(DisplayStyle=ToolStripItemDisplayStyle.Text,
                             Text="Check particular address")

// Actions 
   (* 1 fetching all hosts *) 
checkAllHosts.Click.Add(fun arg ->   
        let parsedString = Pinging.availableHosts(["VATROSLAVS-W7"])
        System.IO.File.WriteAllLines(path, Pinging.hosts())
        label.Text <- parsedString
        refreshComboBox(cmb)
        )

     (* 2 clicking go button*) 
go.Click.Add(fun arg -> label.Text <- Pinging.availableHosts([address.Text]))
btnSearch.Click.Add(fun arg ->
         match cmb.SelectedItem with
             | null -> alertLabel.Text <- "You must select item"
             | _    -> let selected = cmb.SelectedItem.ToString()
                       let parsedString = Pinging.availableHosts([selected])
                       match parsedString with
                            | "" -> label.Text <- selected + " offline"
                            | _ -> label.Text <- parsedString
         )
        // TODO enter functionality for enter button


// adding items to toolbar
toolbar.Items.Add(new ToolStripLabel("Ping boss:")) |> ignore
toolbar.Items.Add(address) |> ignore
toolbar.Items.Add(go)  |>ignore
toolbarControls.Items.Add(alertLabel)  |>ignore


let x = 5



let form = new Form(Text="Web Browser", Size=new Size(800, 600))
form.Controls.Add(toolbar)
form.Controls.Add(label)
form.Controls.Add(cmb)  |>ignore
form.Controls.Add(btnSearch)  |>ignore
form.Controls.Add(checkAllHosts)  |>ignore


form.Controls.Add(toolbarControls)

form.PerformLayout()
form.Show()

Application.Run(form)

    // return an integer exit code
