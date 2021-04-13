import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Event } from '@angular/router';
import { delay } from 'lodash';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { timeInterval, timeout } from 'rxjs/operators';
import { Evento } from 'src/app/_models/Evento';
import { EventoService } from 'src/app/_services/evento.service';

@Component({
  selector: 'app-evento-edit',
  templateUrl: './eventoEdit.component.html',
  styleUrls: ['./eventoEdit.component.css']
})
export class EventoEditComponent implements OnInit {

  titulo = 'Editar Evento';
  imagemAtual = '';
  evento: Evento = new Evento;
  imagemURL = 'assets/img/upload.png';
  registerForm: FormGroup;
  file: File;

  get lotes(): FormArray{
    return <FormArray>this.registerForm.get('lotes');
  }

  get redesSociais(): FormArray{
    return <FormArray>this.registerForm.get('redesSociais');
  }

  constructor(
    private eventoService: EventoService
    ,private modalService: BsModalService
    ,private fb: FormBuilder
    ,private localeService: BsLocaleService
    ,private toastr: ToastrService
    ,private router: ActivatedRoute)
    {
      this.localeService.use('pt-br')
     }

  ngOnInit() {
    this.validation();
    this.carregarEvento();
  }

  carregarEvento(){
    var idEvento =  Number(this.router.snapshot.paramMap.get('id'));
    this.eventoService.getEventoById(idEvento)
    .subscribe(
      (evento: Evento) => {
        this.evento = Object.assign({}, evento);
        this.imagemAtual = this.evento.imagemUrl;
        this.imagemURL = "https://localhost:44331/resources/images/" + this.evento.imagemUrl;

        this.evento.imagemUrl = '';
        this.registerForm.patchValue(this.evento);

        this.evento.lotes.forEach(lote =>{
          this.lotes.push(this.criaLote(lote));
        });

        this.evento.redesSociais.forEach(redeSocial =>{
          this.redesSociais.push(this.criaRedeSocial(redeSocial));
        });

      }
    );
  }

  validation(){
    this.registerForm = this.fb.group({
      id: [],
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      imagemURL: [''],
      qtdPessoas: ['', [Validators.required, Validators.max(700)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      lotes: this.fb.array([]),
      redesSociais: this.fb.array([])
    });
  }

  criaLote(lote: any): FormGroup {
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      quantidade: [lote.quantidade, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio],
      dataFim: [lote.dataFim]
    });
  }

  criaRedeSocial(redeSocial: any) : FormGroup{
    return this.fb.group({
      id: [redeSocial.id],
      nome: [redeSocial.nome, Validators.required],
      url: [redeSocial.url, Validators.required]
    });
  }

  adicionarLote(){
    this.lotes.push(this.criaLote({ id: 0 }));
  }

  removerLote(id: number){
    this.lotes.removeAt(id);
  }

  adicionarRedeSocial(){
    this.redesSociais.push(this.criaRedeSocial({ id: 0 }));
  }

  removerRedeSocial(id: number){
    this.redesSociais.removeAt(id);
  }

  async onFileChange(event){
    const reader = FileReader;
    if (event.target.files && event.target.files.length) {
      this.file = event.target.files;
    }
    this.uploadImagem();

    await this.delay(500);

    this.imagemURL = "https://localhost:44331/resources/images/" + this.evento.imagemUrl;
  }
  private delay(ms: number)
  {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
  uploadImagem(){
    if(this.registerForm.get('imagemURL')?.value !== ''){
      const nomeArquivo = this.registerForm.get('imagemURL')?.value.split('\\', 3);
      this.evento.imagemUrl = nomeArquivo[2];
      this.eventoService.postUpload(this.file, nomeArquivo[2]).subscribe();
    }
    else{
      this.evento.imagemUrl = this.imagemAtual;
    }
  }

  salvarEvento(){
    this.evento = Object.assign({id: this.evento.id}, this.registerForm.value);
    this.uploadImagem();
        this.eventoService.putEvento(this.evento).subscribe(
        () => {
          this.toastr.success('Atualizado com sucesso!');
        }, error => {
          this.toastr.error('Erro ao tentar Atualizar: ${error}');
        }
      );
  }
}
