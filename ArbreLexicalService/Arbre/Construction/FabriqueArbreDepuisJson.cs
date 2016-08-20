using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Construction.Dto;
using ArbreLexicalService.Exceptions;
using Common.Exceptions;
using Common.Helpers;
using Common.Ioc;
using Common.Services;
using Newtonsoft.Json.Linq;

namespace ArbreLexicalService.Arbre.Construction
{
    internal class FabriqueArbreDepuisJson : FabriqueArbre, IFabriqueArbreDepuisJson
    {

        #region Private Fields

        private readonly string pathFichierJson;

        #endregion Private Fields

        #region Public Constructors

        public FabriqueArbreDepuisJson(
            IArbreConstruction arbre) : this(arbre, null)
        {
        }

        public FabriqueArbreDepuisJson(
            IArbreConstruction arbre,
            string pathFichierJson) : base(arbre)
        {
            this.pathFichierJson = pathFichierJson;
        }

        #endregion Public Constructors

        #region Public Methods

        public Task ChargerFichierAsync()
        {
            try
            {
                return Task.Run(
                    (Action)ChargerFichier);
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        public Task DeserialiserAsync(
            string jsonStr)
        {
            try
            {
                return Task.Run(() =>
                    Deserialiser(jsonStr));
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void ChargerFichier()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(pathFichierJson) &&
                    File.Exists(pathFichierJson))
                {
                    var jsonStr = File
                        .ReadAllText(pathFichierJson);

                    Deserialiser(
                        jsonStr);
                }
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        private void Deserialiser(
            string jsonStr)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    var jToken = JToken
                        .Parse(jsonStr);

                    var deserialisateur = new Deserialisateur(
                        jToken);

                    elementsConstruction = deserialisateur
                        .Deserialiser(); 
                }
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        #endregion Private Methods

        #region Private Classes

        private class Deserialisateur : ServiceBase, IDeserialisateurElement
        {

            #region Private Fields

            private const string NOM_PROPRIETE_CHEMIN = "chemin";
            private const string NOM_PROPRIETE_CHOIX = "choix";
            private const string NOM_PROPRIETE_ETIQUETTE = "etiquette";
            private const string NOM_PROPRIETE_REFERENCE = "ref";
            private const string NOM_PROPRIETE_REPETITION = "repetition";
            private readonly JToken jTokenRacine;

            #endregion Private Fields

            #region Public Constructors

            public Deserialisateur(
                JToken jToken)
            {
                this.jTokenRacine = jToken;
            }

            #endregion Public Constructors

            #region Public Methods

            public ElementConstructionDto Deserialiser()
            {
                try
                {
                    return Deserialiser(
                        jTokenRacine);
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            #endregion Public Methods

            #region Private Methods

            private ElementConstructionDto Deserialiser(
                JToken jToken)
            {
                try
                {
                    var jArray = jToken as JArray;

                    if (null != jArray)
                    {
                        return DeserialiserSequenceElements(
                            jArray);
                    }
                    else
                    {
                        return DeserialiserElement(
                            jToken as JObject);
                    }
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            private ElementConstructionDto DeserialiserChoixElements(
                JArray jArray)
            {
                try
                {
                    var elements = DeserialiserElements(
                        jArray);

                    return new ChoixElementsConstructionDto(
                        elements);
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            private ElementConstructionDto DeserialiserElement(
                JObject jObject)
            {
                try
                {
                    JToken jItem;

                    if (jObject.TryGetValue(NOM_PROPRIETE_CHEMIN, out jItem) &&
                        jItem.Type == JTokenType.String)
                    {
                        return new ElementCheminConstructionDto(
                            (jItem as JValue)
                                .Value<string>());
                    }
                    else
                    {
                        if (jObject.TryGetValue(NOM_PROPRIETE_CHOIX, out jItem))
                        {
                            return DeserialiserChoixElements(
                                jItem as JArray);
                        }
                        else
                        {
                            if (jObject.TryGetValue(NOM_PROPRIETE_REPETITION, out jItem))
                            {
                                return DeserialiserRepetitionElement(
                                    jItem as JObject);
                            }
                            else
                            {
                                if (jObject.TryGetValue(NOM_PROPRIETE_ETIQUETTE, out jItem))
                                {
                                    return DeserialiserElementEtiquette(
                                        jItem as JObject);
                                }
                                else
                                {
                                    if (jObject.TryGetValue(NOM_PROPRIETE_REFERENCE, out jItem))
                                    {
                                        return new ElementReferenceConstructionDto(
                                            (jItem as JValue).Value<string>());
                                    }
                                }
                            }
                        }
                    }

                    throw new ExceptionTechniqueArbreConstruction();
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            private ElementConstructionDto DeserialiserElementEtiquette(
                JObject jObject)
            {
                try
                {
                    JToken jToken;
                    ElementConstructionDto enfant;

                    if (jObject.TryGetValue("item", out jToken))
                    {
                        enfant = Deserialiser(
                            jToken);
                    }
                    else
                    {
                        enfant = new ElementCheminConstructionDto(
                            string.Empty);
                    }

                    var id = jObject.TryGetValue("id", out jToken) ?
                        jToken.Value<string>() :
                        null;

                    var typeInt = jObject.TryGetValue("type", out jToken) ?
                        jToken.Value<int>() :
                        (int)EnumTypeBlock.Reference;
                    var type = EnumHelper
                        .RecupererEnum<EnumTypeBlock>(
                            typeInt,
                            EnumTypeBlock.Reference);

                    return new ElementEtiquetteConstructionDto(                        
                        type,
                        id,
                        enfant);
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            private ElementConstructionDto[] DeserialiserElements(
                JArray jArray)
            {
                try
                {
                    var elements = new List<ElementConstructionDto>();

                    foreach (var jTokenDansTab in jArray)
                    {
                        var jArrayCourant = jTokenDansTab as JArray;

                        var element = (null != jArrayCourant ?
                            DeserialiserSequenceElements(jArrayCourant) :
                            DeserialiserElement(jTokenDansTab as JObject));

                        elements
                            .Add(element);
                    }

                    return elements
                        .ToArray();
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            private ElementConstructionDto DeserialiserRepetitionElement(
                JObject jObject)
            {
                try
                {
                    JToken jToken;
                    ElementConstructionDto enfant;

                    if (jObject.TryGetValue("item", out jToken))
                    {
                        enfant = Deserialiser(
                            jToken);
                    }
                    else
                    {
                        enfant = new ElementCheminConstructionDto(
                            string.Empty);
                    }

                    var min = jObject.TryGetValue("min", out jToken) ?
                        jToken.Value<int>() :
                        0;
                    var max = jObject.TryGetValue("max", out jToken) ?
                        jToken.Value<int>() :
                        int.MaxValue;

                    return new ElementRepetitionConstructionDto(
                        enfant,
                        min,
                        max);
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }
            private ElementConstructionDto DeserialiserSequenceElements(
                JArray jArray)
            {
                try
                {
                    var elements = DeserialiserElements(
                        jArray);

                    return new SequenceElementsConstructionDto(
                        elements);
                }
                catch (Exception ex)
                {
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            #endregion Private Methods

        }

        #endregion Private Classes

    }
}
