import { formatDate } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Evento } from '../_models/Evento';

@Injectable({
  providedIn: 'root'
})
export class EventoService {

  baseURL = 'https://localhost:44331/evento/';

  constructor(private http: HttpClient) {}

  getAllEvento(): Observable<Evento[]> {

   return this.http.get<Evento[]>(this.baseURL);
  }

  getEventoByTema(tema: string): Observable<Evento[]> {
    return this.http.get<Evento[]>('${this.baseURL}/getByTema/${tema}');
   }

   getEventoById(id: number): Observable<Evento> {
    return this.http.get<Evento>(this.baseURL + id);
   }

   postUpload(file: File, name: string){
     const fileToUpload = <File>file[0];
     const formData = new FormData;
     formData.append('file', fileToUpload, name)
    return this.http.post(this.baseURL + 'upload', formData);
   }

   postEvento(evento: Evento) {
    return this.http.post(this.baseURL, evento);
   }

   putEvento(evento: Evento) {
    return this.http.put(this.baseURL + '?EventoId=' + evento.id, evento);
   }

   deleteEvento(id: number) {
    //return this.http.put(this.baseURL +'?EventoId='+ evento.id, evento);
    return this.http.delete(this.baseURL + '?EventoId=' + id);
   }

}
