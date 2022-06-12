class EducationInputItem extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        if (this.props.item.InputType === "Decimal" || this.props.item.InputType === "Int") {
            return <input className="form-control" type="number" onChange={e => this.props.onChange(e.target.valueAsNumber)} value={this.props.item.Value} disabled={this.props.blocked} name={this.props.item.Name} key={this.props.item.Name} />;
        }
        if (this.props.item.InputType === "Text") {
            return <input className="form-control" onChange={e => this.props.onChange(e.target.value)} value={this.props.item.Value} disabled={this.props.blocked} name={this.props.item.Name} key={this.props.item.Name} />;
        }
        if (this.props.item.InputType === "Reference") {
            return (<select className="form-control" onChange={e => this.props.onChange(e.target.value)} value={this.props.item.Value} disabled={this.props.blocked} name={this.props.item.Name} key={this.props.item.Name}>
                {((!this.props.item.Value) ? <option>-</option> : null)}
                {this.props.item.RefItems.map(i => <option value={i.Code}>{i.Text}</option>)}
            </select>
            );
        }
        if (this.props.item.InputType === "Boolean") {
            return (<select className="form-control" onChange={e => this.props.onChange(e.target.value)} value={this.props.item.Value} disabled={this.props.blocked} name={this.props.item.Name} key={this.props.item.Name}>
                {((!this.props.item.Value) ? <option>-</option> : null)}
                {this.props.item.RefItems.map(i => <option value={i.Code}>{i.Text}</option>)}
            </select>
            );
        }
        if (this.props.item.InputType === "File") {
            //return <input className="form-control" type="file" onChange={e => this.props.onChange(e.target.value)} value={this.props.item.Value} disabled={this.props.blocked} name={this.props.item.Name} key={this.props.item.Name} />;
            return <FileUploadEditor name={this.props.item.Name} files={this.props.item.Value} fileUploadUrl={this.props.item.FileUploadUrl} description="Документы" onChange={e => this.props.onChange(e.target.value)} disabled={this.props.blocked} />
        }
        if (this.props.item.InputType === "Date") {
            return <input className="form-control" type="date" onChange={e => this.props.onChange(e.target.value)} value={this.props.item.Value} disabled={this.props.blocked} name={this.props.item.Name} key={this.props.item.Name} />;
        }

        throw "Не удалось определить редактор данных : " + this.props.item.InputType;
    }
}